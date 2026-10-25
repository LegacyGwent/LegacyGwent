using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberTwentyFourthXavierTests
    {
        [Fact]
        public async Task XavierUsesStrongestAlliedDwarfBaseStrengthRatherThanLatestOrBoostedPower()
        {
            var f = new HeadlessGameFixture();
            var xavier = f.AddCard(f.Game.Player1Index, CardId.XavierMoran, RowPosition.MyHand);
            var strongest = f.AddCard(f.Game.Player1Index, CardId.XavierMoran, RowPosition.MyRow1, 12);
            var latest = f.AddCard(f.Game.Player1Index, CardId.XavierMoran, RowPosition.MyRow1, 3);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.XavierMoran, RowPosition.MyRow1, 25);
            var nonDwarf = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 30);
            latest.Status.HealthStatus = 40;
            await f.SynchronizeClientsAsync();

            foreach (var card in new[] { strongest, latest, enemy, nonDwarf, xavier })
                await f.Game.SendEvent(new AfterUnitDown(card, true, false, (false, false), false, true));
            await xavier.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Equal(12, xavier.Status.HealthStatus);
        }

        [Fact]
        public async Task XavierResetsHisTrackedMaximumAfterTheRound()
        {
            var f = new HeadlessGameFixture();
            var xavier = f.AddCard(f.Game.Player1Index, CardId.XavierMoran, RowPosition.MyDeck);
            var dwarf = f.AddCard(f.Game.Player1Index, CardId.XavierMoran, RowPosition.MyRow1, 15);
            await f.SynchronizeClientsAsync();
            await f.Game.SendEvent(new AfterUnitDown(dwarf, true, false, (false, false), false, true));
            // Check the accumulated value before the reset as well, so a test
            // accidentally using the independent legacy Effect cannot pass on zero.
            await xavier.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(15, xavier.Status.HealthStatus);
            xavier.Status.HealthStatus = 0;
            await f.Game.SendEvent(new AfterRoundOver(1, 0, 0, null));
            await xavier.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Equal(0, xavier.Status.HealthStatus);
        }
    }
}
