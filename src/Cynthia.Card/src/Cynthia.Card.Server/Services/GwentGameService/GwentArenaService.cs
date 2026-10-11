using System;

namespace Cynthia.Card.Server
{
    //Keeps the Arena runs (draft + 9 wins / 3 losses) of all users. Runs are stored in the "arena" collection.
    public class GwentArenaService
    {
        //room password used for Arena matchmaking; Match() refuses it so only ArenaMatch() can join these rooms
        public const string ArenaRoomPassword = "#arena#";

        private readonly GwentDatabaseService _databaseService;
        private readonly Random _random;
        private readonly object _lock = new object();

        public GwentArenaService(GwentDatabaseService databaseService, Random random)
        {
            _databaseService = databaseService;
            _random = random;
        }

        public ArenaRunModel GetRun(string username)
        {
            lock (_lock)
            {
                return _databaseService.QueryArenaRun(username);
            }
        }

        //starts a new run; fails while an unfinished run exists
        public ArenaRunModel StartRun(string username, string playerName)
        {
            lock (_lock)
            {
                var run = _databaseService.QueryArenaRun(username);
                if (run != null && run.State != ArenaRunState.Finished)
                    return null;
                run = new ArenaRunModel { Id = username, PlayerName = playerName };
                run.Choices = ArenaDraft.GenerateChoices(run.PickNumber, _random);
                _databaseService.SaveArenaRun(run);
                return run;
            }
        }

        //picks one of the offered cards and offers the next set (or the leaders / finishes the draft)
        public ArenaRunModel Pick(string username, string cardId)
        {
            lock (_lock)
            {
                var run = _databaseService.QueryArenaRun(username);
                if (run == null || run.State != ArenaRunState.Drafting || !run.Choices.Contains(cardId))
                    return null;
                if (ArenaDraft.IsLeaderPick(run.PickNumber))
                {
                    run.Leader = cardId;
                    run.Choices.Clear();
                    run.State = ArenaRunState.Ready;
                }
                else
                {
                    run.Deck.Add(cardId);
                    run.PickNumber++;
                    run.Choices = ArenaDraft.GenerateChoices(run.PickNumber, _random);
                }
                _databaseService.SaveArenaRun(run);
                return run;
            }
        }

        public bool Abandon(string username)
        {
            lock (_lock)
            {
                if (_databaseService.QueryArenaRun(username) == null)
                    return false;
                _databaseService.RemoveArenaRun(username);
                return true;
            }
        }

        //returns the deck to play with, or null when the user has no playable run
        public DeckModel GetPlayableDeck(string username)
        {
            var run = GetRun(username);
            if (run == null || run.State != ArenaRunState.Ready)
                return null;
            var deck = run.ToDeckModel();
            return ArenaDraft.IsValidArenaDeck(deck) ? deck : null;
        }

        //a draw counts as neither a win nor a loss
        public void RecordResult(string username, string opponentName, GameStatus status)
        {
            if (status == GameStatus.Draw)
                return;
            lock (_lock)
            {
                var run = _databaseService.QueryArenaRun(username);
                if (run == null || run.State != ArenaRunState.Ready)
                    return;
                if (status == GameStatus.Win)
                    run.Wins++;
                else if (status == GameStatus.Lose)
                    run.Losses++;
                run.Opponents.Add(opponentName);
                if (ArenaDraft.IsFinished(run))
                    run.State = ArenaRunState.Finished;
                _databaseService.SaveArenaRun(run);
            }
        }
    }
}
