using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Cynthia.Card;
using Cynthia.Card.Server;

// Focused regression for the surrender / disconnect crown fix on the diy-ai branch. It drives the
// real production settlement lifecycle (GwentServerGame.GameEnd / BigRoundEnd / GameOverExecute /
// Play) without a database host, simulating the wallet rules that matter for the rule: idempotent
// keys, the daily crown cap and the same-opponent suppression. A conceded human match must leave
// the winner with the two crowns a full match win is worth, counted from actually credited crowns.
//
// The AI branch keeps the reward callback fire-and-forget (Action) so round progression is never
// blocked; the production path only enqueues, and RewardSettlementService owns durable idempotency
// and retries. This fixture therefore treats a callback that returns without throwing as accepted,
// exactly like the production enqueue.
static class SurrenderCrownTest
{
    static int failed;
    static int count;

    static void Check(bool ok, string label, object actual = null)
    {
        count++;
        if (!ok) failed++;
        Console.WriteLine((ok ? "PASS " : "FAIL ") + label + (ok || actual == null ? "" : " " + actual));
    }

    static string Leader => GwentMap.CardMap.First(x => x.Value.Group == Cynthia.Card.Group.Leader).Key;
    static string Bronze => GwentMap.CardMap.First(x => x.Value.Group == Cynthia.Card.Group.Copper).Key;
    static DeckModel Deck() => new DeckModel { Leader = Leader, Deck = new List<string> { Bronze } };
    static Player Sink() => new SinkPlayer { Deck = Deck() };
    static Player Broken() => new BrokenSinkPlayer { Deck = Deck() };

    // Mirrors the persisted wallet rules: a key settles at most once, the same-opponent rule can
    // suppress the whole match, and only six crowns can be credited per day.
    sealed class Wallet
    {
        public const int DailyCrownCap = 6;
        readonly HashSet<string> processed = new HashSet<string>();
        public readonly List<string> Awarded = new List<string>();
        public int Crowns;
        public bool SameOpponentBlocked;
        public bool Award(string key)
        {
            if (!processed.Add(key)) return false;
            if (SameOpponentBlocked || Crowns >= DailyCrownCap) return false;
            Crowns++;
            Awarded.Add(key);
            return true;
        }
    }

    static GwentServerGame Game(out Wallet[] wallets, List<GameResult> results = null,
        string matchId = "match", Player first = null, Player second = null)
    {
        var ledgers = new[] { new Wallet(), new Wallet() };
        wallets = ledgers;
        var game = new GwentServerGame(first ?? Sink(), second ?? Sink(),
            new GwentCardDataService(), r => results?.Add(r), false, matchId);
        game.RoundWon = (winner, key, settled) => { ledgers[winner].Award(key); };
        return game;
    }

    static void WinRound(GwentServerGame game, int playerIndex, int strength = 10)
    {
        var card = game.PlayersDeck[playerIndex][0];
        card.Status.Conceal = false;
        card.Status.Strength = strength;
        game.PlayersPlace[playerIndex][0].Add(card);
    }

    static async Task Main()
    {
        foreach (bool surrender in new[] { true, false })
        {
            string mode = surrender ? "surrender" : "disconnect";
            // A match that is conceded before any round settled owes the winner both crowns.
            var game = Game(out var wallets, matchId: "early-" + mode);
            await game.GameEnd(0, surrender ? new Exception("surrender fixture") : null, surrender);
            Check(wallets[0].Crowns == 2, mode + " in the first undecided round credits both match-win crowns",
                new { winner = wallets[0].Crowns, loser = wallets[1].Crowns });
            Check(wallets[1].Crowns == 0, mode + " credits nothing to the conceding player");
            Check(wallets[0].Awarded.Count == 2 && wallets[0].Awarded.Distinct().Count() == 2,
                mode + " uses two distinct idempotent settlement keys");

            // One genuinely won round already credited a crown; the concession completes the pair.
            game = Game(out wallets, matchId: "one-" + mode);
            WinRound(game, 0);
            await game.BigRoundEnd();
            Check(wallets[0].Crowns == 1, mode + " fixture: the settled round credited exactly one crown");
            var firstCrownKey = wallets[0].Awarded.Single();
            await game.GameEnd(0, new Exception(mode + " in round two"), surrender);
            Check(wallets[0].Crowns == 2 && wallets[0].Awarded.Count == 2 && wallets[0].Awarded[1] != firstCrownKey,
                mode + " in a later round tops the winner up to the two-crown match win");
            Check(wallets[1].Crowns == 0, mode + " does not reward the losing side");

            // A drawn round advances both win counts but produces no crown, so it must not shorten
            // the top-up: the winner still reaches two credited crowns.
            game = Game(out wallets, matchId: "draw-" + mode);
            WinRound(game, 0);
            WinRound(game, 1);
            await game.BigRoundEnd();
            Check(wallets[0].Crowns == 0 && wallets[1].Crowns == 0,
                mode + " fixture: a drawn round credits no crown to either player");
            await game.GameEnd(0, new Exception(mode + " after a draw"), surrender);
            Check(wallets[0].Crowns == 2, mode + " after a drawn round still reaches two credited crowns");
        }

        {
            // The losing side keeps the crown it already won and receives nothing extra.
            var game = Game(out var wallets, matchId: "loser-keeps");
            WinRound(game, 1);
            await game.BigRoundEnd();
            Check(wallets[1].Crowns == 1, "fixture: the eventual loser banked one crown before leaving");
            await game.GameEnd(0, new Exception("loser leaves"), true);
            Check(wallets[0].Crowns == 2 && wallets[1].Crowns == 1,
                "the conceded match tops up only the winner and preserves the loser's progress");
        }

        {
            // Repeated and simultaneous terminal calls publish one result and pay once.
            var results = new List<GameResult>();
            var game = Game(out var wallets, results, "duplicate");
            await game.GameEnd(0, new Exception("leave"), true);
            await game.GameEnd(0, new Exception("duplicate leave"), true);
            await game.GameEnd(1, new Exception("opponent also left"), true);
            Check(results.Count == 1 && wallets[0].Crowns == 2 && wallets[1].Crowns == 0,
                "duplicate GameEnd calls publish one result and cannot pay a second time",
                new { results = results.Count, first = wallets[0].Crowns, second = wallets[1].Crowns });
            Check(game.TempGameResult != null && game.TempGameResult.RedPlayerGameResultStatus == (game.RedCoin[0] == 0 ? GameStatus.Win : GameStatus.Lose),
                "the first declared outcome stands for every later leave");
        }

        {
            // Both players dropping at the same instant race the settlement gate.
            for (int attempt = 0; attempt < 8; attempt++)
            {
                var results = new List<GameResult>();
                var game = Game(out var wallets, results, "race-leave");
                await Task.WhenAll(Enumerable.Range(0, 24).Select(i => Task.Run(() =>
                    game.GameEnd(i % 2, new Exception("both left"), true))));
                var total = wallets[0].Crowns + wallets[1].Crowns;
                if (results.Count != 1 || total != 2)
                {
                    Check(false, "simultaneous leaves publish one result and one match-win reward",
                        new { attempt, results = results.Count, total });
                    goto simultaneousDone;
                }
            }
            Check(true, "simultaneous leaves publish one result and one match-win reward");
        simultaneousDone:;
        }

        {
            // A natural finish must never be flipped or re-rewarded by a late surrender.
            var results = new List<GameResult>();
            var game = Game(out var wallets, results, "natural");
            game.PlayersWinCount[0] = 1;
            WinRound(game, 0);
            await game.BigRoundEnd();// winner reaches two wins, the match is decided
            await game.GameOverExecute();
            var published = results.Count;
            var publishedStatus = game.TempGameResult.RedPlayerGameResultStatus;
            await game.GameEnd(1, new Exception("late surrender"), true);
            Check(results.Count == published && game.TempGameResult.RedPlayerGameResultStatus == publishedStatus,
                "a late surrender does not overwrite an already published natural result");
            Check(wallets[0].Crowns == 1 && wallets[1].Crowns == 0,
                "an already decided match is not topped up for a round that was never played",
                new { first = wallets[0].Crowns, second = wallets[1].Crowns });
        }

        {
            // Concession first, then the abandoned PlayGame reaching GameOverExecute: one result.
            var results = new List<GameResult>();
            var game = Game(out var wallets, results, "concede-first");
            await game.GameEnd(0, new Exception("leave"), true);
            await game.GameOverExecute();
            Check(results.Count == 1 && wallets[0].Crowns == 2,
                "a residual natural-finish call after a concession cannot republish or repay");
        }

        {
            // A round being scored races the concession. The winner must end with exactly the two
            // crowns a match win is worth, never three.
            var results = new List<GameResult>();
            var game = Game(out var wallets, results, "round-race");
            WinRound(game, 0);
            await Task.WhenAll(game.BigRoundEnd(), game.GameEnd(0, new Exception("simultaneous leave"), true));
            Check(results.Count == 1 && wallets[0].Crowns == 2,
                "a round settling while the opponent leaves still yields exactly two crowns",
                new { results = results.Count, crowns = wallets[0].Crowns });
        }

        {
            // The deciding round racing the concession must never exceed two crowns either.
            for (int attempt = 0; attempt < 8; attempt++)
            {
                var results = new List<GameResult>();
                var game = Game(out var wallets, results, "deciding-race");
                game.PlayersWinCount[0] = 1;
                WinRound(game, 0);
                await Task.WhenAll(game.BigRoundEnd(), game.GameEnd(0, new Exception("simultaneous leave"), true));
                if (results.Count != 1 || wallets[0].Crowns > 2)
                {
                    Check(false, "a deciding round racing the concession never overpays",
                        new { attempt, results = results.Count, crowns = wallets[0].Crowns });
                    goto decidingDone;
                }
            }
            Check(true, "a deciding round racing the concession never overpays");
        decidingDone:;
        }

        {
            // A dead peer must not swallow the reward, block Play() or leave the match stuck.
            var first = new BrokenSinkPlayer { Deck = Deck(), Fail = false };
            var second = new BrokenSinkPlayer { Deck = Deck(), Fail = false };
            var game = Game(out var wallets, matchId: "broken", first: first, second: second);
            var play = game.Play();
            first.Fail = second.Fail = true;
            await game.GameEnd(0, new Exception("peer connection lost"));
            var completed = await Task.WhenAny(play, Task.Delay(5000));
            Check(completed == play, "a conceded match with unreachable peers still releases Play()");
            if (completed == play) await play; // Completion alone must not hide a faulted match task.
            Check(wallets[0].Crowns == 2, "the crown is committed before the unreachable peer is notified");
        }

        {
            // The daily cap and the same-opponent rule are applied by the wallet, and the top-up
            // must compose with them instead of overpaying.
            var game = Game(out var wallets, matchId: "capped");
            wallets[0].Crowns = Wallet.DailyCrownCap - 1;
            await game.GameEnd(0, new Exception("leave"), true);
            Check(wallets[0].Crowns == Wallet.DailyCrownCap,
                "the daily crown cap still bounds the concession top-up", new { crowns = wallets[0].Crowns });

            game = Game(out wallets, matchId: "same-opponent");
            wallets[0].SameOpponentBlocked = true;
            await game.GameEnd(0, new Exception("leave"), true);
            Check(wallets[0].Crowns == 0, "the same-opponent daily restriction still suppresses the top-up");
        }

        {
            // AI / self matches and any game without the human reward callback must never award.
            var game = new GwentServerGame(Sink(), Sink(), new GwentCardDataService(), r => { }, false, "ai");
            Exception error = null;
            try { await game.GameEnd(0, new Exception("ai or self game"), true); }
            catch (Exception e) { error = e; }
            Check(error == null && game.TempGameResult != null,
                "a game without the human reward callback ends without touching crowns");
        }

        {
            // A committed round whose enqueue threw must be retried with the same key, never a third.
            var game = Game(out var wallets, matchId: "ambiguous-commit");
            WinRound(game, 0);
            bool loseConfirmation = true;
            game.RoundWon = (w, key, time) =>
            {
                wallets[w].Award(key);
                if (loseConfirmation) throw new Exception("commit succeeded but confirmation was lost");
            };
            await game.BigRoundEnd();
            loseConfirmation = false;
            await game.GameEnd(0, null);
            Check(wallets[0].Crowns == 2 && wallets[0].Awarded.Count == 2,
                "ambiguous committed round retries reuse the old key rather than creating a third crown");
        }
        {
            var results = new List<GameResult>();
            var game = Game(out var wallets, results, "late-before-publication");
            game.PlayersWinCount[0] = 2;
            await game.GameEnd(1, new Exception("decided winner disconnects"));
            await game.GameOverExecute();
            Check(results.Count == 1 && results[0].RedPlayerGameResultStatus ==
                (game.RedCoin[0] == 0 ? GameStatus.Win : GameStatus.Lose) && wallets.Sum(x => x.Crowns) == 0,
                "deciding score is preserved even if leave arrives before natural result publication");
        }
        {
            // A terminal call whose reward enqueue is still in flight must hold every other end
            // caller and Play() until the single owner finishes.
            var results = new List<GameResult>();
            var game = Game(out var wallets, results, "pending-duplicate");
            var entered = new ManualResetEventSlim(false);
            var release = new ManualResetEventSlim(false);
            game.RoundWon = (w, key, time) => { entered.Set(); release.Wait(); wallets[w].Award(key); };
            var play = game.Play();
            var first = Task.Run(() => game.GameEnd(0, null));
            Check(entered.Wait(5000), "the settlement owner reached the pending reward");
            var duplicate = game.GameEnd(1, null);
            Check(!duplicate.IsCompleted && !play.IsCompleted && results.Count == 0,
                "duplicate end does not release gameplay before pending rewards finish");
            release.Set();
            var all = Task.WhenAll(first, duplicate, play);
            Check(await Task.WhenAny(all, Task.Delay(5000)) == all, "pending settlement and both end callers complete");
            await all;
            Check(results.Count == 1 && wallets[0].Crowns == 2 && wallets[1].Crowns == 0,
                "only the settlement owner publishes the result and releases gameplay");
        }

        Console.WriteLine("COMPLETE checks=" + count + " failed=" + failed);
        Environment.ExitCode = failed == 0 ? 0 : 1;
    }
}

class SinkPlayer : Player
{
    public SinkPlayer() { _downstream.Receive += _ => Task.CompletedTask; }
}

// The player's client side rejects every game message, mirroring a peer that dropped mid-match.
class BrokenSinkPlayer : Player
{
    public bool Fail = true;
    public BrokenSinkPlayer() { _downstream.Receive += _ => Fail ? Task.FromException(new Exception("connection lost")) : Task.CompletedTask; }
}
