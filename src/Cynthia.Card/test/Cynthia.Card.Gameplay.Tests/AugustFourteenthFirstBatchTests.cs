using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustFourteenthFirstBatchTests
    {
        [Fact]
        public async Task BrokilonGirlCreatesDoomedCopiesInBothOtherRows()
        {
            var fixture = new HeadlessGameFixture();
            var girl = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GirlWhoDrankBrokilonWater, RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await girl.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            var copies = fixture.Game.GetPlaceCards(fixture.Game.Player1Index)
                .Where(card => card != girl && card.Status.CardId == CardId.GirlWhoDrankBrokilonWater)
                .ToArray();
            Assert.Equal(2, copies.Length);
            Assert.Contains(copies, card => card.Status.CardRow == RowPosition.MyRow1 && card.Status.IsDoomed);
            Assert.Contains(copies, card => card.Status.CardRow == RowPosition.MyRow3 && card.Status.IsDoomed);
        }

        [Fact]
        public async Task BrokilonGirlTransformsIntoStrongestSameRowDryadOnOwnerTurnOnly()
        {
            var fixture = new HeadlessGameFixture();
            var girl = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GirlWhoDrankBrokilonWater, RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Braenn, RowPosition.MyRow1, 8);
            var strongest = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DryadMatron, RowPosition.MyRow1, 12);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.Equal(CardId.GirlWhoDrankBrokilonWater, girl.Status.CardId);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(strongest.Status.CardId, girl.Status.CardId);
        }

        [Fact]
        public async Task BrokilonGirlEndsCleanlyWithoutAnotherSameRowDryad()
        {
            var fixture = new HeadlessGameFixture();
            var girl = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GirlWhoDrankBrokilonWater, RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));

            Assert.Equal(CardId.GirlWhoDrankBrokilonWater, girl.Status.CardId);
        }

        [Fact]
        public async Task SerritDamagesUnlockedSurvivorWithoutReducingItToOne()
        {
            var fixture = new HeadlessGameFixture();
            var serrit = fixture.AddCard(fixture.Game.Player1Index, CardId.Serrit, RowPosition.MyRow1);
            var target = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 12);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await serrit.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(5, target.CardPoint());
        }

        [Fact]
        public async Task SerritReducesLockedSurvivorToOneAfterDamage()
        {
            var fixture = new HeadlessGameFixture();
            var serrit = fixture.AddCard(fixture.Game.Player1Index, CardId.Serrit, RowPosition.MyRow1);
            var target = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 12);
            target.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await serrit.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(1, target.CardPoint());
            Assert.True(target.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task SerritDoesNotRunPowerReductionAfterLethalDamage()
        {
            var fixture = new HeadlessGameFixture();
            var serrit = fixture.AddCard(fixture.Game.Player1Index, CardId.Serrit, RowPosition.MyRow1);
            var target = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 7);
            target.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await serrit.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.False(target.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task SerritSetsRevealedEnemyHandUnitToOne()
        {
            var fixture = new HeadlessGameFixture();
            var serrit = fixture.AddCard(fixture.Game.Player1Index, CardId.Serrit, RowPosition.MyRow1);
            var target = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, 12);
            target.Status.IsReveal = true;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await serrit.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(1, target.CardPoint());
            Assert.Equal(RowPosition.MyHand, target.Status.CardRow);
        }
    }
}
