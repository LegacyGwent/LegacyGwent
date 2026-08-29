using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustNinthFirstBatchTests
    {
        [Fact]
        public async Task WraithSorcererLockCyclePausesWhileLocked()
        {
            var fixture = new HeadlessGameFixture();
            var sorcerer = fixture.AddCard(
                fixture.Game.Player1Index, CardId.WraithSorcerer, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new OnGameStart());
            Assert.True(sorcerer.Status.IsLock);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(sorcerer.Status.IsLock);

            sorcerer.Status.IsLock = false;
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.False(sorcerer.Status.IsLock);
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(sorcerer.Status.IsLock);
        }

        [Fact]
        public async Task WraithSorcererDoesNotToggleBelowThreePower()
        {
            var fixture = new HeadlessGameFixture();
            var sorcerer = fixture.AddCard(
                fixture.Game.Player1Index, CardId.WraithSorcerer, RowPosition.MyRow1, 2);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));

            Assert.False(sorcerer.Status.IsLock);
        }

        [Fact]
        public async Task GaelRepeatsOnlyTheDrainAfterTwoOwnerTurns()
        {
            var fixture = new HeadlessGameFixture();
            var gael = fixture.AddCard(fixture.Game.Player1Index, "70146", RowPosition.MyRow2);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            await gael.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(-1, enemy.Status.HealthStatus);
            Assert.Equal(1, gael.Status.HealthStatus);
            Assert.Equal(RowStatus.GoldenFroth,
                fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);

            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][1]
                .SetStatus<BitingFrostStatus>();
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));

            Assert.Equal(2, gael.Status.HealthStatus);
            Assert.Equal(RowStatus.BitingFrost,
                fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);
        }

        [Fact]
        public async Task SvalblodUsesInitialStrengthForBothOverlappingRules()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var svalblod = fixture.AddCard(fixture.Game.Player1Index, "70099", RowPosition.MyRow1);
            var deckTwo = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck, 2);
            var deckThree = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck, 3);
            var handTwo = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Lambert, RowPosition.MyHand, 2);
            await fixture.SynchronizeClientsAsync();

            await svalblod.Effect.CardPlayEffect(false, false);

            Assert.Contains(deckTwo, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Equal(4, deckTwo.Status.Strength);
            Assert.Equal(0, deckTwo.Status.HealthStatus);
            Assert.True(deckThree.Status.CardRow.IsInDeck());
            Assert.Equal(5, deckThree.Status.Strength);
            Assert.Equal(-2, deckThree.Status.HealthStatus);
            Assert.True(handTwo.Status.CardRow.IsInHand());
            Assert.Equal(4, handTwo.Status.Strength);
            Assert.Equal(-2, handTwo.Status.HealthStatus);
        }

        [Fact]
        public async Task CrowmotherGeneratesOtherRowsAndResurrectsOnlySmallCrowsIntoItsRow()
        {
            var fixture = new HeadlessGameFixture();
            var mother = fixture.AddCard(fixture.Game.Player1Index, "70159", RowPosition.MyRow2);
            var smallCrow = fixture.AddCard(fixture.Game.Player1Index, "70136", RowPosition.MyCemetery, 2);
            var largeCrow = fixture.AddCard(fixture.Game.Player1Index, "70136", RowPosition.MyCemetery, 3);
            await fixture.SynchronizeClientsAsync();

            await mother.Effect.CardPlayEffect(false, false);

            Assert.False(mother.Status.IsDoomed);
            Assert.Contains(fixture.Game.PlayersPlace[fixture.Game.Player1Index][0],
                card => card.Status.CardId == "70136");
            Assert.Contains(fixture.Game.PlayersPlace[fixture.Game.Player1Index][2],
                card => card.Status.CardId == "70136");
            Assert.Contains(smallCrow, fixture.Game.PlayersPlace[fixture.Game.Player1Index][1]);
            Assert.Contains(largeCrow, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
        }

        [Fact]
        public async Task DeafeningSirenAddsTwoBottomCopiesAndSummonsOnlyForEnemyRainOnOwnerTurn()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var siren = fixture.AddCard(fixture.Game.Player1Index, "70116", RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new OnGameStart());
            Assert.Equal(2, fixture.Game.PlayersDeck[fixture.Game.Player1Index]
                .Count(card => card.Status.CardId == "70116"));

            fixture.Game.GameRound = (TwoPlayer)fixture.Game.Player2Index;
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][0]
                .SetStatus<TorrentialRainStatus>();
            Assert.True(siren.Status.CardRow.IsInCemetery());

            fixture.Game.GameRound = (TwoPlayer)fixture.Game.Player1Index;
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][1]
                .SetStatus<TorrentialRainStatus>();
            Assert.True(siren.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task SvalblodBrawlerCountsAllHazardsAndBoostsOnlyActualPowerLost()
        {
            var fixture = new HeadlessGameFixture();
            var brawler = fixture.AddCard(fixture.Game.Player1Index, "70092", RowPosition.MyRow1);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 3);
            await fixture.Game.GameRowEffect[fixture.Game.Player1Index][0]
                .SetStatus<BitingFrostStatus>();
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][2]
                .SetStatus<ImpenetrableFogStatus>();
            await fixture.SynchronizeClientsAsync();

            await brawler.Effect.CardPlayEffect(false, false);

            Assert.Contains(enemy, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
            Assert.Equal(3, brawler.Status.HealthStatus);
        }

        [Fact]
        public async Task VildkaarlBoostsByActualLossIncludingLethalAdjacentDamage()
        {
            var fixture = new HeadlessGameFixture();
            var left = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 2);
            var vildkaarl = fixture.AddCard(
                fixture.Game.Player1Index, "70160", RowPosition.MyRow1);
            var right = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyRow1, 8);
            await fixture.SynchronizeClientsAsync();

            await vildkaarl.Effect.CardPlayEffect(false, false);

            Assert.Contains(left, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Equal(4, right.CardPoint());
            Assert.Equal(6, vildkaarl.Status.HealthStatus);
        }

        [Fact]
        public async Task TuirseachWarshipNeverTargetsItselfAsTheOnlyHealthyUnit()
        {
            var fixture = new HeadlessGameFixture();
            var ship = fixture.AddCard(fixture.Game.Player1Index, "70096", RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(0, ship.Status.HealthStatus);
        }
    }
}
