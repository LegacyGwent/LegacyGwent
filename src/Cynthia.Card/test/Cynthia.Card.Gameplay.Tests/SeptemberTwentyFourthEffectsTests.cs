using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberTwentyFourthEffectsTests
    {
        [Fact]
        public async Task BroniborUsesTheTotalArmorActuallyGrantedAsDamage()
        {
            var f = new HeadlessGameFixture();
            var infantry = f.AddCard(f.Game.Player1Index, CardId.PoorFIngInfantry, RowPosition.MyRow1);
            var soldier = f.AddCard(f.Game.Player1Index, "44001", RowPosition.MyRow1);
            var bronibor = f.AddCard(f.Game.Player1Index, CardId.Bronibor, RowPosition.MyRow3);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            f.FirstPlayer.QueueRows(RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            await f.Game.SendEvent(new AfterUnitDown(
                infantry, false, false, (false, false), false));

            Assert.Equal(1, infantry.Status.Armor);
            Assert.Equal(1, soldier.Status.Armor);
            Assert.Equal(0, bronibor.Status.Armor);
            Assert.Equal(-2, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task DamnationRestoresInitialArmorAfterSummoningItsBronzeUnits()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var tridam = f.AddCard(f.Game.Player1Index, "44001", RowPosition.MyDeck);
            var nilfgaardianKnight = f.AddCard(f.Game.Player1Index, "34004", RowPosition.MyDeck);
            var damnation = f.AddCard(f.Game.Player1Index, "70078", RowPosition.MyHand);
            f.FirstPlayer.QueueRows(RowPosition.MyRow2);
            await f.SynchronizeClientsAsync();

            await damnation.Effect.CardUseEffect();

            Assert.Equal(RowPosition.MyRow2, tridam.Status.CardRow);
            Assert.Equal(4, tridam.Status.Armor);
            Assert.Equal(RowPosition.MyRow2, nilfgaardianKnight.Status.CardRow);
            Assert.Equal(2, nilfgaardianKnight.Status.Armor);
        }

        [Fact]
        public async Task LyrianArbalestDamagesLowerPowerOrDestroysAllArmor()
        {
            var damageFixture = new HeadlessGameFixture();
            var arbalest = damageFixture.AddCard(
                damageFixture.Game.Player1Index, "70095", RowPosition.MyRow1, 8);
            var weaker = damageFixture.AddCard(
                damageFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 5);
            await damageFixture.SynchronizeClientsAsync();
            await arbalest.Effect.CardPlayEffect(false, false);
            Assert.Equal(-3, weaker.Status.HealthStatus);

            var armorFixture = new HeadlessGameFixture();
            var secondArbalest = armorFixture.AddCard(
                armorFixture.Game.Player1Index, "70095", RowPosition.MyRow1, 8);
            var stronger = armorFixture.AddCard(
                armorFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 10);
            stronger.Status.Armor = 4;
            await armorFixture.SynchronizeClientsAsync();
            await secondArbalest.Effect.CardPlayEffect(false, false);
            Assert.Equal(0, stronger.Status.Armor);
            Assert.Equal(0, stronger.Status.HealthStatus);
        }

        [Fact]
        public async Task VlodimirPlaysTheWeakestEligibleWitcherFromDeck()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var eskel = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            f.AddCard(f.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var vlodimir = f.AddCard(
                f.Game.Player1Index, CardId.VlodimirVonEverec, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();

            var played = await vlodimir.Effect.CardPlayEffect(false, false);

            Assert.Equal(1, played);
            Assert.Same(eskel, Assert.Single(f.Game.PlayersStay[f.Game.Player1Index]));
        }

        [Fact]
        public async Task OphelieLowersDeckPowerDealsTheLossAndHealsAfterALethalHit()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            var deckUnit = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck, 5);
            var woundedAlly = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 6);
            woundedAlly.Status.HealthStatus = -2;
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Nekker, RowPosition.MyRow1, 4);
            var ophelie = f.AddCard(
                f.Game.Player1Index, CardId.OphelieVanMoorlehem, RowPosition.MyRow2);
            f.FirstPlayer.QueueMenuCardIds(CardId.Eskel, CardId.Wolf);
            f.FirstPlayer.PlaceSelectionOverride = info => info.CanSelect.CardsPartToLocation()
                .Where(location => location.RowPosition.IsOnPlace() &&
                                   !location.RowPosition.IsMyRow())
                .Take(info.SelectCount)
                .ToList();
            await f.SynchronizeClientsAsync();

            await ophelie.Effect.CardPlayEffect(false, false);

            Assert.Equal(1, deckUnit.CardPoint());
            Assert.True(enemy.IsDead || !enemy.Status.CardRow.IsOnPlace());
            Assert.Equal(0, woundedAlly.Status.HealthStatus);
        }

        [Fact]
        public async Task SkjordalDiscardsThenQueuesADifferentBronzeFromTheSameClan()
        {
            var f = new HeadlessGameFixture();
            f.Game.PlayersDeck[f.Game.Player1Index].Clear();
            f.Game.PlayersCemetery[f.Game.Player1Index].Clear();
            var discarded = f.AddCard(f.Game.Player1Index, "64011", RowPosition.MyDeck);
            var resurrected = f.AddCard(f.Game.Player1Index, "64025", RowPosition.MyCemetery);
            var skjordal = f.AddCard(
                f.Game.Player1Index, CardId.SkjordalDrummond, RowPosition.MyRow1);
            f.FirstPlayer.QueueMenuCardIds("64011", "64025");
            await f.SynchronizeClientsAsync();

            var queued = await skjordal.Effect.CardPlayEffect(false, false);

            Assert.Equal(1, queued);
            Assert.True(discarded.Status.CardRow.IsInCemetery());
            Assert.Same(resurrected, Assert.Single(f.Game.PlayersStay[f.Game.Player1Index]));
        }

        [Fact]
        public async Task GezrasStrengthensThenQueuesTheSameWitcherForReplay()
        {
            var f = new HeadlessGameFixture();
            var target = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyRow1);
            var initialStrength = target.Status.Strength;
            target.Status.HealthStatus = 2;
            target.Status.Armor = 3;
            var gezras = f.AddCard(
                f.Game.Player1Index, CardId.GezrasOfLeyda, RowPosition.MyRow2);
            f.FirstPlayer.QueueMenuOptionKeys("GezrasOfLeyda_1_Strengthen");
            await f.SynchronizeClientsAsync();

            var queued = await gezras.Effect.CardPlayEffect(false, false);

            Assert.Equal(1, queued);
            Assert.Equal(initialStrength + 1, target.Status.Strength);
            Assert.Equal(0, target.Status.HealthStatus);
            Assert.Equal(0, target.Status.Armor);
            Assert.Same(target, Assert.Single(f.Game.PlayersStay[f.Game.Player1Index]));
        }

        [Fact]
        public async Task GaetanRecountsTheOpposingRowForEachRepeat()
        {
            var f = new HeadlessGameFixture();
            var ally = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 10);
            var gaetan = f.AddCard(f.Game.Player1Index, CardId.Gaetan, RowPosition.MyRow1);
            var enemies = Enumerable.Range(0, 4)
                .Select(_ => f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20))
                .ToArray();
            await f.SynchronizeClientsAsync();

            await gaetan.Effect.CardPlayEffect(false, false);

            Assert.Equal(-1, ally.Status.HealthStatus);
            Assert.Equal(-4, enemies.Sum(enemy => enemy.Status.HealthStatus));
        }

        [Fact]
        public async Task BrehenStrengthensByHalfTheActualAlliedDamageThenHitsTheOpposingRow()
        {
            var f = new HeadlessGameFixture();
            var firstAlly = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 10);
            var brehen = f.AddCard(f.Game.Player1Index, CardId.Brehen, RowPosition.MyRow1);
            var secondAlly = f.AddCard(f.Game.Player1Index, CardId.Nekker, RowPosition.MyRow1, 10);
            var enemies = Enumerable.Range(0, 2)
                .Select(_ => f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20))
                .ToArray();
            await f.SynchronizeClientsAsync();

            await brehen.Effect.CardPlayEffect(false, false);

            Assert.Equal(-1, firstAlly.Status.HealthStatus);
            Assert.Equal(-1, secondAlly.Status.HealthStatus);
            Assert.Equal(7, brehen.Status.Strength);
            Assert.All(enemies, enemy => Assert.Equal(-5, enemy.Status.HealthStatus));
        }

        [Fact]
        public async Task CatSchoolWitcherThugMovesItsTargetAndRecalculatesBeforeTheRepeat()
        {
            var f = new HeadlessGameFixture();
            var thug = f.AddCard(
                f.Game.Player1Index, CardId.CatSchoolWitcherThug, RowPosition.MyRow2);
            var target = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            for (var index = 0; index < 3; index++)
            {
                f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 20);
            }
            await f.SynchronizeClientsAsync();

            await thug.Effect.CardPlayEffect(false, false);

            Assert.Equal(RowPosition.MyRow2, target.Status.CardRow);
            Assert.Equal(-8, target.Status.HealthStatus);
        }

        [Fact]
        public async Task CatSchoolWitcherDamagesEveryUnitOnTheOpposingRowByTheDifference()
        {
            var f = new HeadlessGameFixture();
            var cat = f.AddCard(
                f.Game.Player1Index, CardId.CatSchoolWitcher, RowPosition.MyRow2);
            var first = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 10);
            var second = f.AddCard(f.Game.Player2Index, CardId.Nekker, RowPosition.MyRow2, 10);
            f.AddCard(f.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1, 10);
            await f.SynchronizeClientsAsync();

            await cat.Effect.CardPlayEffect(false, false);

            Assert.Equal(-4, first.Status.HealthStatus);
            Assert.Equal(-4, second.Status.HealthStatus);
        }
    }
}
