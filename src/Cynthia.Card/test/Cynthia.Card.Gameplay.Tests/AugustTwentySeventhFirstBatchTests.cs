using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustTwentySeventhFirstBatchTests
    {
        [Fact]
        public async Task EndregaEggsCreatesANormalCopyOnItsLeft()
        {
            var fixture = new HeadlessGameFixture();
            var egg = fixture.AddCard(
                fixture.Game.Player1Index, CardId.EndregaEggs, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await egg.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            var row = fixture.Game.PlayersPlace[fixture.Game.Player1Index][0];
            Assert.Equal(2, row.Count(card => card.Status.CardId == CardId.EndregaEggs));
            var copy = row[egg.GetLocation().CardIndex - 1];
            Assert.Equal(CardId.EndregaEggs, copy.Status.CardId);
            Assert.False(copy.Status.IsDoomed);
            Assert.Equal(3, copy.Status.Countdown);
        }

        [Fact]
        public async Task EndregaEggDeathwishCreatesLarvaAndSummonsStrengthenedQueenAtTen()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var queen = fixture.AddCard(
                fixture.Game.Player1Index, CardId.EndregaQueen, RowPosition.MyDeck, 9);
            var egg = fixture.AddCard(
                fixture.Game.Player1Index, CardId.EndregaEggs, RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () => await egg.Effect.ToCemetery());

            Assert.Contains(egg, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Single(
                fixture.Game.PlayersPlace[fixture.Game.Player1Index][1],
                card => card.Status.CardId == CardId.EndregaLarva);
            Assert.Equal(10, queen.Status.Strength);
            Assert.Equal(RowPosition.MyRow1, queen.Status.CardRow);
        }

        [Fact]
        public async Task LockedEggPausesCountdownAndDoesNotTriggerDeathwishWhenDestroyed()
        {
            var fixture = new HeadlessGameFixture();
            var egg = fixture.AddCard(
                fixture.Game.Player1Index, CardId.EndregaEggs, RowPosition.MyRow1);
            egg.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(3, egg.Status.Countdown);

            await fixture.Game.AddTask(async () => await egg.Effect.ToCemetery());

            Assert.DoesNotContain(
                fixture.Game.PlayersPlace[fixture.Game.Player1Index].SelectMany(row => row),
                card => card.Status.CardId == CardId.EndregaLarva);
        }

        [Fact]
        public async Task EndregaLarvaTransformsAfterThreeUnlockedOwnerTurnEnds()
        {
            var fixture = new HeadlessGameFixture();
            var larva = fixture.AddCard(
                fixture.Game.Player1Index, CardId.EndregaLarva, RowPosition.MyRow1);
            larva.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(CardId.EndregaLarva, larva.Status.CardId);
            Assert.Equal(3, larva.Status.Countdown);

            larva.Status.IsLock = false;
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(CardId.EndregaLarva, larva.Status.CardId);
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(CardId.EndregaWarrior, larva.Status.CardId);
        }

        [Fact]
        public async Task EndregaQueenMovesToDeckBottomAndCreatesOneEggEveryThreeOwnerTurnEnds()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var queen = fixture.AddCard(
                fixture.Game.Player1Index, CardId.EndregaQueen, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new OnGameStart());
            Assert.Same(queen, fixture.Game.PlayersDeck[fixture.Game.Player1Index].Last());

            await queen.Effect.Summon(new CardLocation(RowPosition.MyRow3, 0), queen);
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.DoesNotContain(
                fixture.Game.PlayersPlace[fixture.Game.Player1Index][2],
                card => card.Status.CardId == CardId.EndregaEggs);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            var row = fixture.Game.PlayersPlace[fixture.Game.Player1Index][2];
            var egg = Assert.Single(row, card => card.Status.CardId == CardId.EndregaEggs);
            Assert.Equal(queen.GetLocation().CardIndex - 1, egg.GetLocation().CardIndex);
            Assert.Equal(3, egg.Status.Countdown);
            Assert.False(egg.Status.IsDoomed);
        }

        [Fact]
        public async Task CalantheCannotPlayASpyFromDeck()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var spy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            spy.Status.IsSpying = true;
            var normal = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ArachasHatchling, RowPosition.MyDeck);
            var target = fixture.AddCard(
                fixture.Game.Player1Index, CardId.AnCraiteGreatsword, RowPosition.MyRow1);
            var calanthe = fixture.AddCard(
                fixture.Game.Player1Index, CardId.QueenCalanthe, RowPosition.MyHand);
            fixture.FirstPlayer.QueueMenuCardIds(normal.Status.CardId);
            await fixture.SynchronizeClientsAsync();

            await calanthe.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));

            Assert.Contains(spy, fixture.Game.PlayersDeck[fixture.Game.Player1Index]);
            Assert.True(spy.Status.IsSpying);
            Assert.Equal(2, fixture.FirstPlayer.LastMenuOptionCount);
        }
    }
}
