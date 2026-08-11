using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustEleventhSecondBatchTests
    {
        [Fact]
        public async Task WarCouncilRoundPlayResolvesDeepGateChainWithoutFloatingCards()
        {
            var fixture = new HeadlessGameFixture();
            ClearMutableZones(fixture);

            var warCouncil = fixture.AddCard(
                fixture.Game.Player1Index, CardId.WarCouncil, RowPosition.MyHand);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.CeallachDyffryn, RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Recruit, RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.MagneDivision, RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Ointment, RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.Recruit, RowPosition.MyCemetery);
            var enemyDraw = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck);

            fixture.FirstPlayer.QueueMenuCardIds(
                CardId.CeallachDyffryn,
                CardId.Emissary,
                CardId.Recruit,
                CardId.Recruit);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.RoundPlayCard(
                fixture.Game.Player1Index,
                new RoundInfo
                {
                    HandCardIndex = fixture.Game.PlayersHandCard[fixture.Game.Player1Index]
                        .IndexOf(warCouncil),
                    CardLocation = new CardLocation(RowPosition.SpecialPlace, 0)
                });

            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player2Index]);
            Assert.False(fixture.Game.OperactionList.IsRunning);
            Assert.Contains(
                fixture.Game.PlayersHandCard[fixture.Game.Player1Index],
                card => card.Status.CardId == CardId.BattlePreparation);
            Assert.Contains(enemyDraw, fixture.Game.PlayersHandCard[fixture.Game.Player2Index]);
            Assert.True(enemyDraw.Status.IsReveal);
            Assert.Contains(warCouncil, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Contains(
                fixture.Game.GetAllCard(fixture.Game.Player1Index, isContainDead: true, isHasConceal: true),
                card => card.Status.CardId == CardId.CeallachDyffryn && card.Status.CardRow.IsOnPlace());
            Assert.Contains(
                fixture.Game.GetAllCard(fixture.Game.Player1Index, isContainDead: true, isHasConceal: true),
                card => card.Status.CardId == CardId.MagneDivision && card.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task WarCouncilSwapsSelectedHandCardAndRevealsEnemyCopperDraw()
        {
            var fixture = new HeadlessGameFixture();
            ClearMutableZones(fixture);

            var warCouncil = fixture.AddCard(
                fixture.Game.Player1Index, CardId.WarCouncil, RowPosition.MyHand);
            var handCard = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            var deckCard = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            var enemyDraw = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck);
            fixture.FirstPlayer.QueueMenuCardIds(CardId.GeraltOfRivia);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.RoundPlayCard(
                fixture.Game.Player1Index,
                new RoundInfo
                {
                    HandCardIndex = fixture.Game.PlayersHandCard[fixture.Game.Player1Index]
                        .IndexOf(warCouncil),
                    CardLocation = new CardLocation(RowPosition.SpecialPlace, 0)
                });

            Assert.Contains(handCard, fixture.Game.PlayersDeck[fixture.Game.Player1Index]);
            Assert.Contains(deckCard, fixture.Game.PlayersHandCard[fixture.Game.Player1Index]);
            Assert.Contains(
                fixture.Game.PlayersHandCard[fixture.Game.Player1Index],
                card => card.Status.CardId == CardId.BattlePreparation);
            Assert.Contains(enemyDraw, fixture.Game.PlayersHandCard[fixture.Game.Player2Index]);
            Assert.True(enemyDraw.Status.IsReveal);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player2Index]);
            Assert.False(fixture.Game.OperactionList.IsRunning);
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
