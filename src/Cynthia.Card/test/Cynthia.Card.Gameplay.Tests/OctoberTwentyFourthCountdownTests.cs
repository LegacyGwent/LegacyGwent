using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberTwentyFourthCountdownTests
    {
        private const string Queen = "70213";

        [Fact]
        public async Task QueenTicksOtherInsectoidsOnlyEverySecondOwnerTurnStart()
        {
            var f = new HeadlessGameFixture();
            var queen = f.AddCard(f.Game.Player1Index, Queen, RowPosition.MyHand);
            var egg = f.AddCard(f.Game.Player1Index, CardId.EndregaEggs, RowPosition.MyRow2);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.EndregaEggs, RowPosition.MyRow1);
            var deck = f.AddCard(f.Game.Player1Index, CardId.EndregaEggs, RowPosition.MyDeck);
            var nonInsect = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2);
            nonInsect.Status.Countdown = 3;
            nonInsect.Status.IsCountdown = true;
            await f.SynchronizeClientsAsync();
            await queen.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));
            Assert.True(queen.Status.IsImmue);
            Assert.Equal(2, queen.Status.Countdown);
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player2Index));
            Assert.Equal(2, queen.Status.Countdown);
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            Assert.Equal(1, queen.Status.Countdown);
            Assert.Equal(3, egg.Status.Countdown);
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            Assert.Equal(2, queen.Status.Countdown);
            Assert.Equal(2, egg.Status.Countdown);
            Assert.Equal(3, enemy.Status.Countdown);
            Assert.Equal(3, deck.Status.Countdown);
            Assert.Equal(3, nonInsect.Status.Countdown);
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(1, egg.Status.Countdown);
        }

        [Fact]
        public async Task CountdownTriggerResolvesDestroyTransformAndSpawnWithoutSyntheticTurnEvents()
        {
            var f = new HeadlessGameFixture();
            var queen = f.AddCard(f.Game.Player1Index, Queen, RowPosition.MyRow1);
            var egg = f.AddCard(f.Game.Player1Index, CardId.EndregaEggs, RowPosition.MyRow1);
            var larva = f.AddCard(f.Game.Player1Index, CardId.EndregaLarva, RowPosition.MyRow2);
            var endrega = f.AddCard(f.Game.Player1Index, CardId.EndregaQueen, RowPosition.MyRow3);
            var behemoth = f.AddCard(f.Game.Player1Index, CardId.ArachasBehemoth, RowPosition.MyRow3);
            var locked = f.AddCard(f.Game.Player1Index, CardId.EndregaLarva, RowPosition.MyRow2);
            foreach (var card in new[] { queen, egg, larva, endrega, behemoth, locked })
            {
                card.Status.Countdown = 1;
                card.Status.IsCountdown = true;
            }
            locked.Status.IsLock = true;
            await f.SynchronizeClientsAsync();
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            Assert.Contains(egg, f.Game.PlayersCemetery[f.Game.Player1Index]);
            Assert.Equal(CardId.EndregaWarrior, larva.Status.CardId);
            Assert.Equal(3, endrega.Status.Countdown);
            Assert.Equal(0, behemoth.Status.Countdown);
            Assert.Equal(1, locked.Status.Countdown);
            Assert.Equal(CardId.EndregaLarva, locked.Status.CardId);
            var hatchling = Assert.Single(f.Game.GetPlaceCards(f.Game.Player1Index),
                card => card.Status.CardId == CardId.ArachasHatchling);
            var generatedEgg = Assert.Single(f.Game.GetPlaceCards(f.Game.Player1Index),
                card => card.Status.CardId == CardId.EndregaEggs);
            Assert.Equal(3, generatedEgg.Status.Countdown);
            // The egg's Deathwish larva was not in the recipient snapshot.
            Assert.Contains(f.Game.GetPlaceCards(f.Game.Player1Index), card =>
                card != locked && card.Status.CardId == CardId.EndregaLarva && card.Status.Countdown == 3);
            await f.Game.SendEvent(new TriggerCountdown(behemoth));
            Assert.Single(f.Game.GetPlaceCards(f.Game.Player1Index),
                card => card.Status.CardId == CardId.ArachasHatchling);
        }

        [Fact]
        public async Task LockedQueenPausesItsOwnCountdown()
        {
            var f = new HeadlessGameFixture();
            var queen = f.AddCard(f.Game.Player1Index, Queen, RowPosition.MyRow1);
            var larva = f.AddCard(f.Game.Player1Index, CardId.EndregaLarva, RowPosition.MyRow2);
            queen.Status.Countdown = 1;
            queen.Status.IsLock = true;
            await f.SynchronizeClientsAsync();
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            Assert.Equal(1, queen.Status.Countdown);
            Assert.Equal(3, larva.Status.Countdown);
        }

        [Fact]
        public async Task AnotherQueenReceivesAnExtraCountdownStep()
        {
            var f = new HeadlessGameFixture();
            var first = f.AddCard(f.Game.Player1Index, Queen, RowPosition.MyRow1);
            var second = f.AddCard(f.Game.Player1Index, Queen, RowPosition.MyRow2);
            first.Status.Countdown = 1;
            await f.SynchronizeClientsAsync();
            await f.Game.SendEvent(new TriggerCountdown(first));
            Assert.Equal(2, first.Status.Countdown);
            Assert.Equal(1, second.Status.Countdown);
        }
    }
}
