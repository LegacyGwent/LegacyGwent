using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberFifthFirstBatchTests
    {
        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        public async Task AlpDrainsOnlyTheStrongestVisibleUnitInTheOppositeRow(int power)
        {
            var f = new HeadlessGameFixture();
            var alp = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow2);
            var weak = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 4);
            var strong = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, power);
            var otherRow = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var hidden = f.AddCard(f.Game.Player2Index, CardId.Morenn, RowPosition.MyRow2, 20);
            hidden.Status.Conceal = true;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index)));

            Assert.Equal(-1, strong.Status.HealthStatus);
            Assert.Equal(1, alp.Status.HealthStatus);
            Assert.Equal(0, weak.Status.HealthStatus);
            Assert.Equal(0, otherRow.Status.HealthStatus);
            Assert.Equal(0, hidden.Status.HealthStatus);
        }

        [Theory]
        [InlineData(8, false, false, false)]
        [InlineData(7, true, false, false)]
        [InlineData(7, false, true, false)]
        [InlineData(7, false, false, true)]
        public async Task AlpDoesNotDrainWhenBlockedOrOutsideItsOwnTurn(
            int power, bool locked, bool enemyTurn, bool inHand)
        {
            var f = new HeadlessGameFixture();
            var alp = f.AddCard(f.Game.Player1Index, CardId.Alp,
                inHand ? RowPosition.MyHand : RowPosition.MyRow1);
            alp.Status.IsLock = locked;
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, power);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () => await f.Game.SendEvent(
                new AfterTurnOver(enemyTurn ? f.Game.Player2Index : f.Game.Player1Index)));
            Assert.Equal(0, enemy.Status.HealthStatus);
            Assert.Equal(0, alp.Status.HealthStatus);
        }

        [Fact]
        public async Task AlpUsesCurrentPowerAndDrainsOnlyOneTiedStrongest()
        {
            var f = new HeadlessGameFixture();
            var alp = f.AddCard(f.Game.Player1Index, CardId.Alp, RowPosition.MyRow1);
            alp.Status.HealthStatus = 2;
            var enemies = Enumerable.Range(0, 2).Select(_ =>
                f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 8)).ToArray();
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () =>
                await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index)));
            Assert.Equal(-1, enemies.Sum(card => card.Status.HealthStatus));
            Assert.Equal(3, alp.Status.HealthStatus);
        }

        [Fact]
        public async Task SwampReturnsAllOwnFogletsToBottomBeforeApplyingChosenFog()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            f.Game.PlayersCemetery[f.Game.Player1Index].Clear();
            var swamp = f.AddCard(f.Game.Player1Index, CardId.TheThingInTheSwamp, RowPosition.MyRow1);
            var top = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var corpses = Enumerable.Range(0, 5).Select(_ =>
                f.AddCard(f.Game.Player1Index, CardId.Foglet, RowPosition.MyCemetery)).ToArray();
            var unrelated = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var enemyCorpse = f.AddCard(f.Game.Player2Index, CardId.Foglet, RowPosition.MyCemetery);
            f.FirstPlayer.QueueRows(RowPosition.EnemyRow3);
            f.FirstPlayer.BeforeRowSelection = () =>
            {
                Assert.Equal(new[] { top }.Concat(corpses), f.Game.PlayersDeck[f.Game.Player1Index]);
                Assert.All(corpses, card => Assert.Equal(RowPosition.MyDeck, card.Status.CardRow));
            };
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
                await swamp.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowStatus.ImpenetrableFog, f.Game.GameRowEffect[f.Game.Player2Index][2].RowStatus);
            Assert.Equal(RowStatus.None, f.Game.GameRowEffect[f.Game.Player2Index][0].RowStatus);
            Assert.Equal(RowPosition.MyCemetery, unrelated.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, enemyCorpse.Status.CardRow);
            Assert.Empty(f.FirstPlayer.MenuRequests);
            Assert.Single(f.FirstPlayer.RowRequests);
        }

        [Theory]
        [InlineData(false, false, false, false, RowStatus.TorrentialRain, true)]
        [InlineData(true, false, false, false, RowStatus.TorrentialRain, false)]
        [InlineData(false, true, false, false, RowStatus.TorrentialRain, false)]
        [InlineData(false, false, true, false, RowStatus.TorrentialRain, false)]
        [InlineData(false, false, false, true, RowStatus.TorrentialRain, false)]
        [InlineData(false, false, false, false, RowStatus.BitingFrost, false)]
        public async Task SwampRepeatsOnlyForEnemyRainDuringOwnersTurn(
            bool enemyTurn, bool ownHalf, bool locked, bool inHand, RowStatus weather, bool triggers)
        {
            var f = new HeadlessGameFixture();
            var swamp = f.AddCard(f.Game.Player1Index, CardId.TheThingInTheSwamp,
                inHand ? RowPosition.MyHand : RowPosition.MyRow1);
            swamp.Status.IsLock = locked;
            f.Game.GameRound = (TwoPlayer)(enemyTurn ? f.Game.Player2Index : f.Game.Player1Index);
            f.FirstPlayer.QueueRows(RowPosition.EnemyRow3);
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () => await f.Game.GameRowEffect[
                ownHalf ? f.Game.Player1Index : f.Game.Player2Index][1].SetStatus(weather));

            Assert.Equal(triggers ? 1 : 0, f.FirstPlayer.RowRequests.Count);
            Assert.Equal(triggers ? RowStatus.ImpenetrableFog : RowStatus.None,
                f.Game.GameRowEffect[f.Game.Player2Index][2].RowStatus);
        }

        [Fact]
        public async Task SwampCanRepeatOnEachNewRainWithoutRecursingOnItsOwnFog()
        {
            var f = new HeadlessGameFixture();
            f.AddCard(f.Game.Player1Index, CardId.TheThingInTheSwamp, RowPosition.MyRow1);
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            f.FirstPlayer.QueueRows(RowPosition.EnemyRow2, RowPosition.EnemyRow3);
            await f.SynchronizeClientsAsync();
            for (var i = 0; i < 2; i++)
                await f.Game.AddTask(async () =>
                    await f.Game.GameRowEffect[f.Game.Player2Index][0].SetStatus<TorrentialRainStatus>());
            Assert.Equal(2, f.FirstPlayer.RowRequests.Count);
            Assert.Equal(RowStatus.ImpenetrableFog, f.Game.GameRowEffect[f.Game.Player2Index][1].RowStatus);
            Assert.Equal(RowStatus.ImpenetrableFog, f.Game.GameRowEffect[f.Game.Player2Index][2].RowStatus);
        }

        [Fact]
        public async Task MilaenCountsOnlyUnturnedFriendlyAmbushUnitsOnBoard()
        {
            var f = new HeadlessGameFixture();
            var milaen = f.AddCard(f.Game.Player1Index, CardId.Milaen, RowPosition.MyRow1);
            f.AddCard(f.Game.Player1Index, CardId.Morenn, RowPosition.MyRow1).Status.Conceal = true;
            f.AddCard(f.Game.Player1Index, CardId.Toruviel, RowPosition.MyRow2).Status.Conceal = true;
            f.AddCard(f.Game.Player1Index, CardId.Morenn, RowPosition.MyRow3);
            f.AddCard(f.Game.Player1Index, CardId.Morenn, RowPosition.MyHand).Status.Conceal = true;
            f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow3).Status.Conceal = true;
            f.AddCard(f.Game.Player2Index, CardId.Morenn, RowPosition.MyRow1).Status.Conceal = true;
            var targets = Enumerable.Range(0, 3).Select(_ =>
                f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 20)).ToArray();
            f.FirstPlayer.QueueRows(RowPosition.EnemyRow2);
            await f.SynchronizeClientsAsync();

            await milaen.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Equal(new[] { -8, 0, -8 }, targets.Select(card => card.Status.HealthStatus));
        }

        [Fact]
        public async Task MilaenHitsASingleEndpointOnlyOnceAndRetainsEnemyRowSelection()
        {
            var f = new HeadlessGameFixture();
            var milaen = f.AddCard(f.Game.Player1Index, CardId.Milaen, RowPosition.MyRow1);
            var target = f.AddCard(f.Game.Player2Index,
                CardId.Wolf, RowPosition.MyRow3, 20);
            f.FirstPlayer.QueueRows(RowPosition.EnemyRow3);
            await f.SynchronizeClientsAsync();
            await milaen.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(-6, target.Status.HealthStatus);
            Assert.All(Assert.Single(f.FirstPlayer.RowRequests), row => Assert.False(row.IsMyRow()));
        }

        [Fact]
        public async Task SihilOffersAllBronzeSilverUnitsAndPlaysTheChosenOne()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var sihil = f.AddCard(f.Game.Player1Index, CardId.Sihil, RowPosition.MyStay);
            var bronze = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var silver = f.AddCard(f.Game.Player1Index, CardId.Milaen, RowPosition.MyDeck);
            f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            f.AddCard(f.Game.Player1Index, CardId.Swallow, RowPosition.MyDeck);
            f.FirstPlayer.QueueMenuOptionKeys("Sihil_3_PlayUnit");
            f.FirstPlayer.QueueMenuCardIds(CardId.Milaen);
            await f.SynchronizeClientsAsync();
            var followup = await sihil.Effect.CardUseEffect();
            Assert.Equal(1, followup);
            Assert.Equal(RowPosition.MyStay, silver.Status.CardRow);
            Assert.Equal(RowPosition.MyDeck, bronze.Status.CardRow);
            Assert.Equal(new[] { CardId.Wolf, CardId.Milaen },
                f.FirstPlayer.MenuRequests.Last().SelectList.Select(card => card.CardId));
        }

        [Fact]
        public async Task SihilSafelyHandlesDeckWithNoEligibleUnits()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var sihil = f.AddCard(f.Game.Player1Index, CardId.Sihil, RowPosition.MyStay);
            f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            f.FirstPlayer.QueueMenuOptionKeys("Sihil_3_PlayUnit");
            await f.SynchronizeClientsAsync();
            Assert.Equal(0, await sihil.Effect.CardUseEffect());
            Assert.Single(f.FirstPlayer.MenuRequests);
        }

        [Theory]
        [InlineData("Sihil_1_DamageOdd", -3, 0)]
        [InlineData("Sihil_2_DamageEven", 0, -3)]
        public async Task SihilRetainsBothDamageOptions(string option, int oddDamage, int evenDamage)
        {
            var f = new HeadlessGameFixture();
            var sihil = f.AddCard(f.Game.Player1Index, CardId.Sihil, RowPosition.MyStay);
            var odd = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 7);
            var even = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 8);
            f.FirstPlayer.QueueMenuOptionKeys(option);
            await f.SynchronizeClientsAsync();
            Assert.Equal(0, await sihil.Effect.CardUseEffect());
            Assert.Equal(oddDamage, odd.Status.HealthStatus);
            Assert.Equal(evenDamage, even.Status.HealthStatus);
        }

        [Fact]
        public async Task WeavessOptionsRetainDistinctLocaleKeysAcrossTheWire()
        {
            var f = new HeadlessGameFixture();
            var weavess = f.AddCard(f.Game.Player1Index, CardId.WeavessIncantation, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();
            await weavess.Effects.RaiseEvent(new CardPlayEffect(false, false));
            var menu = Assert.Single(f.FirstPlayer.MenuRequests);
            Assert.Equal(new[] { "WeavessIncantation_1_Strenghten", "WeavessIncantation_2_PlayRelict" },
                menu.SelectList.Select(card => card.Info));
        }
    }
}
