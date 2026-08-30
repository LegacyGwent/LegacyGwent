using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustThirtiethSecondBatchTests
    {
        [Fact]
        public async Task CoenDoesNotBoostAtTurnStartOrEnemyTurnEnd()
        {
            var fixture = new HeadlessGameFixture();
            var witcher = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 5);
            fixture.AddCard(fixture.Game.Player1Index, "70158", RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));

            Assert.Equal(0, witcher.Status.HealthStatus);
        }

        [Fact]
        public async Task CoenRecalculatesTheWeakestWitchersBetweenItsTwoBoosts()
        {
            var fixture = new HeadlessGameFixture();
            var weakest = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 5);
            var nextWeakest = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 6);
            var stronger = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Lambert, RowPosition.MyRow3, 9);
            fixture.AddCard(fixture.Game.Player1Index, "70158", RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(2, weakest.Status.HealthStatus);
            Assert.Equal(1, nextWeakest.Status.HealthStatus);
            Assert.Equal(0, stronger.Status.HealthStatus);
        }

        [Fact]
        public async Task CoenBoostsEveryTiedWeakestWitcherTwiceAndExcludesSelf()
        {
            var fixture = new HeadlessGameFixture();
            var tiedOne = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 5);
            var tiedTwo = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 5);
            var coen = fixture.AddCard(
                fixture.Game.Player1Index, "70158", RowPosition.MyRow2, 1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(2, tiedOne.Status.HealthStatus);
            Assert.Equal(2, tiedTwo.Status.HealthStatus);
            Assert.Equal(0, coen.Status.HealthStatus);
        }
    }
}
