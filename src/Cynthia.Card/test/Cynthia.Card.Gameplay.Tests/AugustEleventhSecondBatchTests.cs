using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustEleventhSecondBatchTests
    {
        [Fact]
        public async Task CerysFearlessRestoresDiyContractAndStopsAfterThreeUses()
        {
            var fixture = new HeadlessGameFixture();
            ClearMutableZones(fixture);

            var cerys = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CerysFearless, RowPosition.MyRow1);
            cerys.Status.HealthStatus = 10;
            var discardSource = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CursedScroll, RowPosition.MyRow1);
            var discarded = new[]
            {
                fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand),
                fixture.AddCard(fixture.Game.Player1Index, CardId.ArachasHatchling, RowPosition.MyHand),
                fixture.AddCard(fixture.Game.Player1Index, "15011", RowPosition.MyHand),
                fixture.AddCard(fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyHand)
            };
            await fixture.SynchronizeClientsAsync();
            await cerys.Effects.RaiseEvent(new CardPlayEffect(false, false));

            foreach (var card in discarded)
            {
                await fixture.Game.AddTask(() => card.Effect.Discard(discardSource));
            }

            Assert.All(discarded.Take(3), card => Assert.NotEqual(RowPosition.MyCemetery, card.Status.CardRow));
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.Contains(discarded[3], fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Equal(0, cerys.Status.Countdown);
            Assert.Equal(-2, cerys.Status.HealthStatus);
            Assert.True(cerys.Status.CardRow.IsOnPlace());
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player2Index]);
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task CerysFearlessIgnoresGoldDiscardAndHostileNonSpySource()
        {
            var fixture = new HeadlessGameFixture();
            ClearMutableZones(fixture);

            var cerys = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CerysFearless, RowPosition.MyRow1);
            var friendlySource = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CursedScroll, RowPosition.MyRow1);
            var hostileSource = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1);
            var gold = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            var copper = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();
            await cerys.Effects.RaiseEvent(new CardPlayEffect(false, false));

            await fixture.Game.AddTask(() => gold.Effect.Discard(friendlySource));
            await fixture.Game.AddTask(() => copper.Effect.Discard(hostileSource));

            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.Equal(3, cerys.Status.Countdown);
            Assert.Equal(0, cerys.Status.HealthStatus);
            Assert.False(gold.Status.CardRow.IsOnPlace());
            Assert.False(copper.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task BowDryadUsesThreeDamageOnDeployAndOwnTurnMoveToRangedRow()
        {
            var fixture = new HeadlessGameFixture();
            var dryad = fixture.AddCard(
                fixture.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow2);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            await fixture.SynchronizeClientsAsync();

            await dryad.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(-3, enemy.Status.HealthStatus);

            fixture.Game.GameRound = (TwoPlayer)fixture.Game.Player1Index;
            await fixture.Game.SendEvent(new AfterCardMove(dryad, dryad));
            Assert.Equal(-6, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task AppearanceTriggersApplyToSummonedOrResurrectedUnits()
        {
            var fixture = new HeadlessGameFixture();
            var pillager = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DrummondPillager, RowPosition.MyRow1);
            var armoredCavalry = fixture.AddCard(
                fixture.Game.Player1Index, CardId.AlbaArmoredCavalry, RowPosition.MyRow1);
            var siegeSupport = fixture.AddCard(
                fixture.Game.Player1Index, CardId.SiegeSupport, RowPosition.MyRow1);
            var smuggler = fixture.AddCard(
                fixture.Game.Player1Index, CardId.HawkerSmuggler, RowPosition.MyRow1);
            var bear = fixture.AddCard(
                fixture.Game.Player1Index, CardId.SavageBear, RowPosition.MyRow1);
            var clanDrummond = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2, 10);
            clanDrummond.Status.Categories = new[] { Categorie.ClanDrummond };
            var enemyArrival = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 10);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterUnitDown(
                clanDrummond,
                isFromHand: false,
                isFromPlance: false,
                isMoveInfo: (false, false),
                isSpying: false,
                isPlayed: false));
            await fixture.Game.SendEvent(new AfterUnitDown(
                enemyArrival,
                isFromHand: false,
                isFromPlance: false,
                isMoveInfo: (false, false),
                isSpying: false,
                isPlayed: false));

            Assert.Equal(11, clanDrummond.Status.Strength);
            Assert.Equal(1, clanDrummond.Status.HealthStatus);
            Assert.Equal(1, armoredCavalry.Status.HealthStatus);
            Assert.Equal(1, smuggler.Status.HealthStatus);
            Assert.Equal(-1, enemyArrival.Status.HealthStatus);
            Assert.True(pillager.Status.CardRow.IsOnPlace());
            Assert.True(siegeSupport.Status.CardRow.IsOnPlace());
            Assert.True(bear.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task DamnedSorceressDealsSixWithOneCursedAllyAndScalesByEachExtra()
        {
            var oneCursed = new HeadlessGameFixture();
            var firstSorceress = oneCursed.AddCard(
                oneCursed.Game.Player1Index, CardId.DamnedSorceress, RowPosition.MyRow1, 20);
            var firstCursed = oneCursed.AddCard(
                oneCursed.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            firstCursed.Status.Categories = new[] { Categorie.Cursed };
            await oneCursed.SynchronizeClientsAsync();
            await firstSorceress.Effect.CardDownEffect(false, false);
            Assert.Equal(-6, firstCursed.Status.HealthStatus);

            var twoCursed = new HeadlessGameFixture();
            var secondSorceress = twoCursed.AddCard(
                twoCursed.Game.Player1Index, CardId.DamnedSorceress, RowPosition.MyRow1, 20);
            var secondCursed = twoCursed.AddCard(
                twoCursed.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            secondCursed.Status.Categories = new[] { Categorie.Cursed };
            var thirdCursed = twoCursed.AddCard(
                twoCursed.Game.Player1Index, CardId.ArachasHatchling, RowPosition.MyRow1, 20);
            thirdCursed.Status.Categories = new[] { Categorie.Cursed };
            await twoCursed.SynchronizeClientsAsync();
            await secondSorceress.Effect.CardDownEffect(false, false);
            Assert.Equal(-7, secondCursed.Status.HealthStatus);
        }

        private static void ClearMutableZones(HeadlessGameFixture fixture)
        {
            foreach (var playerIndex in new[] { fixture.Game.Player1Index, fixture.Game.Player2Index })
            {
                fixture.Game.PlayersDeck[playerIndex].Clear();
                fixture.Game.PlayersHandCard[playerIndex].Clear();
                fixture.Game.PlayersCemetery[playerIndex].Clear();
                fixture.Game.PlayersStay[playerIndex].Clear();
                foreach (var row in fixture.Game.PlayersPlace[playerIndex])
                {
                    row.Clear();
                }
            }
        }
    }
}
