using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustFourthSecondBatchTests
    {
        [Fact]
        public async Task GasconMovesOtherUnitsButNeverMovesItself()
        {
            var fixture = new HeadlessGameFixture();
            var gascon = fixture.AddCard(
                fixture.Game.Player1Index, "70032", RowPosition.MyRow1, strength: 10);
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await gascon.Effect.CardPlayEffect(false, false);

            Assert.Equal(RowPosition.MyRow1, gascon.Status.CardRow);
            Assert.NotEqual(RowPosition.MyRow1, ally.Status.CardRow);
            Assert.Equal(-2, gascon.Status.HealthStatus);
        }

        [Fact]
        public async Task AlbastraSpawnsArmoredWingsAndRepeatsOppositeFrostEveryTwoTurns()
        {
            var fixture = new HeadlessGameFixture();
            var albastra = fixture.AddCard(
                fixture.Game.Player1Index, "70180", RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await albastra.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));

            var wings = fixture.Game.GetPlaceCards(fixture.Game.Player1Index)
                .Where(card => card.Status.CardId == "70181" || card.Status.CardId == "70182")
                .ToList();
            Assert.True(albastra.Status.IsImmue);
            Assert.Equal(2, wings.Count);
            Assert.All(wings, wing => Assert.Equal(3, wing.Status.Armor));
            Assert.Equal(
                RowStatus.BitingFrost,
                fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);

            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].SetStatus<NoneStatus>();
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(RowStatus.None, fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(
                RowStatus.BitingFrost,
                fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);
        }

        [Fact]
        public async Task SyannaDealsHerCurrentWoundedAmountEverySecondOwnerTurnEnd()
        {
            var fixture = new HeadlessGameFixture();
            var target = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var syanna = fixture.AddCard(fixture.Game.Player1Index, "70025", RowPosition.MyRow2);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();
            await syanna.Effect.CardPlayEffect(false, false);
            await syanna.Effect.Damage(4, enemy);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(0, target.Status.HealthStatus);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(-4, target.Status.HealthStatus);
        }

        [Fact]
        public async Task CoenDamagesOnDeployAndBoostsAllOtherTiedWeakestWitchers()
        {
            var fixture = new HeadlessGameFixture();
            var target = fixture.AddCard(
                fixture.Game.Player1Index, CardId.RedanianKnight, RowPosition.MyRow1);
            var weakestOne = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 5);
            var weakestTwo = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, strength: 5);
            var stronger = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Lambert, RowPosition.MyRow3, strength: 8);
            var coen = fixture.AddCard(fixture.Game.Player1Index, "70158", RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            await coen.Effect.CardPlayEffect(false, false);
            Assert.Equal(-3, target.Status.HealthStatus);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(2, weakestOne.Status.HealthStatus);
            Assert.Equal(2, weakestTwo.Status.HealthStatus);
            Assert.Equal(0, stronger.Status.HealthStatus);
            Assert.Equal(0, coen.Status.HealthStatus);
        }

        [Fact]
        public async Task CoenDeathwishSpawnsFarmerOnTheOppositeEnemyRow()
        {
            var fixture = new HeadlessGameFixture();
            var coen = fixture.AddCard(fixture.Game.Player1Index, "70158", RowPosition.MyRow2);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await coen.Effect.Damage(20, enemy);

            var farmers = fixture.Game.GetPlaceCards(fixture.Game.Player2Index, RowPosition.MyRow2)
                .Where(card => card.Status.CardId == "15011")
                .ToList();
            Assert.Single(farmers);
        }
    }
}
