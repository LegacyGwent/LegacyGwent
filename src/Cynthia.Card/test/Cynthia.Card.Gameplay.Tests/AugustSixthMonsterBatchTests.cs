using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustSixthMonsterBatchTests
    {
        [Fact]
        public async Task IgnisFatuusSpawnsOneDoomedCopyWithoutRecursiveDeployment()
        {
            var fixture = new HeadlessGameFixture();
            var ignis = fixture.AddCard(
                fixture.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await ignis.Effects.RaiseEvent(new CardPlayEffect(false, false));

            var copies = fixture.Game.RowToList(fixture.Game.Player1Index, RowPosition.MyRow1)
                .Where(card => card.Status.CardId == CardId.IgnisFatuus)
                .ToList();
            Assert.Equal(2, copies.Count);
            Assert.Single(copies, card => card.Status.IsDoomed);
        }

        [Fact]
        public async Task IgnisFatuusConvertsEnemyFogDamageToWeakenAndPreservesShield()
        {
            var fixture = new HeadlessGameFixture();
            fixture.AddCard(fixture.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow1);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            enemy.Status.IsShield = true;
            await fixture.SynchronizeClientsAsync();

            await enemy.Effect.Damage(2, null, damageType: DamageType.ImpenetrableFog);

            Assert.Equal(8, enemy.Status.Strength);
            Assert.Equal(0, enemy.Status.HealthStatus);
            Assert.True(enemy.Status.IsShield);

            var normalFixture = new HeadlessGameFixture();
            var normalEnemy = normalFixture.AddCard(
                normalFixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            normalEnemy.Status.IsShield = true;
            await normalFixture.SynchronizeClientsAsync();

            await normalEnemy.Effect.Damage(2, null, damageType: DamageType.ImpenetrableFog);

            Assert.Equal(10, normalEnemy.Status.Strength);
            Assert.False(normalEnemy.Status.IsShield);
        }

        [Fact]
        public async Task MultipleIgnisFatuusCopiesStillApplyFogWeakenOnlyOnce()
        {
            var fixture = new HeadlessGameFixture();
            fixture.AddCard(fixture.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow2);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            await fixture.SynchronizeClientsAsync();

            await enemy.Effect.Damage(2, null, damageType: DamageType.ImpenetrableFog);

            Assert.Equal(8, enemy.Status.Strength);
        }

        [Fact]
        public async Task LockedOrOffBoardIgnisFatuusDoesNotConvertFogDamage()
        {
            var lockedFixture = new HeadlessGameFixture();
            var lockedIgnis = lockedFixture.AddCard(
                lockedFixture.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow1);
            lockedIgnis.Status.IsLock = true;
            var firstEnemy = lockedFixture.AddCard(
                lockedFixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            await lockedFixture.SynchronizeClientsAsync();

            await firstEnemy.Effect.Damage(2, null, damageType: DamageType.ImpenetrableFog);

            Assert.Equal(10, firstEnemy.Status.Strength);
            Assert.Equal(-2, firstEnemy.Status.HealthStatus);

            var offBoardFixture = new HeadlessGameFixture();
            offBoardFixture.AddCard(
                offBoardFixture.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyCemetery);
            var secondEnemy = offBoardFixture.AddCard(
                offBoardFixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            await offBoardFixture.SynchronizeClientsAsync();

            await secondEnemy.Effect.Damage(2, null, damageType: DamageType.ImpenetrableFog);

            Assert.Equal(10, secondEnemy.Status.Strength);
            Assert.Equal(-2, secondEnemy.Status.HealthStatus);
        }

        [Fact]
        public async Task CloudGiantTriggersOnlyOnItsOwnersTurnEndAndMovesASurvivor()
        {
            var fixture = new HeadlessGameFixture();
            var giant = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CloudGiant, RowPosition.MyRow1, strength: 8);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 10);
            await fixture.SynchronizeClientsAsync();

            await giant.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player2Index));
            Assert.Equal(0, enemy.Status.HealthStatus);
            Assert.Equal(RowPosition.MyRow1, enemy.Status.CardRow);

            await giant.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(-4, enemy.Status.HealthStatus);
            Assert.NotEqual(RowPosition.MyRow1, enemy.Status.CardRow);
        }

        [Fact]
        public async Task GaelDrainsTheOppositeRowOnDeployAndExactlyOnceAfterThreeOwnerTurns()
        {
            var fixture = new HeadlessGameFixture();
            var gael = fixture.AddCard(
                fixture.Game.Player1Index, "70146", RowPosition.MyRow2);
            var enemies = new[]
            {
                fixture.AddCard(fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2),
                fixture.AddCard(fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyRow2)
            };
            await fixture.SynchronizeClientsAsync();

            await gael.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.All(enemies, enemy => Assert.Equal(-2, enemy.Status.HealthStatus));
            Assert.Equal(4, gael.Status.HealthStatus);
            Assert.Equal(
                RowStatus.GoldenFroth,
                fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);

            await gael.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.All(enemies, enemy => Assert.Equal(-2, enemy.Status.HealthStatus));

            await gael.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.All(enemies, enemy => Assert.Equal(-2, enemy.Status.HealthStatus));
            Assert.Equal(4, gael.Status.HealthStatus);

            await gael.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.All(enemies, enemy => Assert.Equal(-2, enemy.Status.HealthStatus));
            Assert.Equal(4, gael.Status.HealthStatus);

            await gael.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.All(enemies, enemy => Assert.Equal(-4, enemy.Status.HealthStatus));
            Assert.Equal(8, gael.Status.HealthStatus);

            await gael.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.All(enemies, enemy => Assert.Equal(-4, enemy.Status.HealthStatus));
        }

        [Fact]
        public async Task RedRiderCountsThreeFrostDeathsButIgnoresRoundCleanup()
        {
            var fixture = new HeadlessGameFixture();
            var rider = fixture.AddCard(
                fixture.Game.Player1Index, "70083", RowPosition.MyDeck);
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][0]
                .SetStatus<BitingFrostStatus>();
            await fixture.SynchronizeClientsAsync();

            var cleanupVictim = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1);
            await rider.Effects.RaiseEvent(new BeforeCardToCemetery(
                cleanupVictim, cleanupVictim.GetLocation(), true));
            Assert.Equal(RowPosition.MyDeck, rider.Status.CardRow);

            for (var i = 0; i < 2; i++)
            {
                var victim = fixture.AddCard(
                    fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1);
                await rider.Effects.RaiseEvent(new BeforeCardToCemetery(
                    victim, victim.GetLocation()));
            }
            Assert.Equal(RowPosition.MyDeck, rider.Status.CardRow);

            var thirdVictim = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1);
            await rider.Effects.RaiseEvent(new BeforeCardToCemetery(
                thirdVictim, thirdVictim.GetLocation()));

            Assert.True(rider.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task WaterHagBoostsNormalAlliesAndStrengthensOgroids()
        {
            var fixture = new HeadlessGameFixture();
            var hag = fixture.AddCard(
                fixture.Game.Player1Index, "70169", RowPosition.MyRow1);
            var normal = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, strength: 5);
            var ogroid = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2, strength: 5);
            var third = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow3, strength: 5);
            ogroid.Status.Categories = new[] { Categorie.Ogroid };
            await fixture.SynchronizeClientsAsync();

            await hag.Effect.CardPlayEffect(false, false);

            Assert.Equal(5, normal.Status.Strength);
            Assert.Equal(1, normal.Status.HealthStatus);
            Assert.Equal(6, ogroid.Status.Strength);
            Assert.Equal(0, ogroid.Status.HealthStatus);
            Assert.Equal(1, third.Status.HealthStatus);
        }

        [Fact]
        public async Task OgreWarriorChecksExactlyOnceAfterThreeOwnerTurnStarts()
        {
            var fixture = new HeadlessGameFixture();
            var warrior = fixture.AddCard(
                fixture.Game.Player1Index, "70168", RowPosition.MyRow1, strength: 8);
            fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, strength: 7);
            await fixture.SynchronizeClientsAsync();

            await warrior.Effects.RaiseEvent(new CardPlayEffect(false, false));
            await warrior.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.Equal(0, warrior.Status.HealthStatus);

            await warrior.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await warrior.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(0, warrior.Status.HealthStatus);

            await warrior.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(4, warrior.Status.HealthStatus);
            Assert.False(warrior.Status.IsCountdown);

            await warrior.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(4, warrior.Status.HealthStatus);
        }

        [Fact]
        public async Task HybridTriggersBronzeDeathwishThenConsumesItsRightNeighborNextTurn()
        {
            var fixture = new HeadlessGameFixture();
            var hybrid = fixture.AddCard(
                fixture.Game.Player1Index, "70176", RowPosition.MyRow1, strength: 6);
            var egg = fixture.AddCard(
                fixture.Game.Player1Index, CardId.HarpyEgg, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await hybrid.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Contains(
                fixture.Game.GetPlaceCards(fixture.Game.Player1Index),
                card => card.Status.CardId == CardId.HarpyHatchling);
            Assert.True(egg.Status.CardRow.IsOnPlace());
            Assert.Equal(0, hybrid.Status.HealthStatus);

            await hybrid.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.True(egg.Status.CardRow.IsOnPlace());

            await hybrid.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.False(egg.Status.CardRow.IsOnPlace());
            Assert.True(hybrid.CardPoint() > hybrid.Status.Strength);
        }

        [Fact]
        public async Task IgnisFatuusDoesNotSuppressApiarianPhantomKillFrost()
        {
            var lethal = new HeadlessGameFixture();
            lethal.AddCard(lethal.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow1);
            var lethalPhantom = lethal.AddCard(
                lethal.Game.Player1Index, "70085", RowPosition.MyRow1);
            await lethal.Game.GameRowEffect[lethal.Game.Player2Index][1]
                .SetStatus<ImpenetrableFogStatus>();
            lethal.AddCard(
                lethal.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, strength: 6);
            await lethal.SynchronizeClientsAsync();

            await lethal.Game.AddTask(async () =>
                await lethalPhantom.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowStatus.BitingFrost, lethal.Game.GameRowEffect[lethal.Game.Player2Index][1].RowStatus);
            Assert.Equal(RowStatus.None, lethal.Game.GameRowEffect[lethal.Game.Player2Index][0].RowStatus);
            Assert.Equal(RowStatus.None, lethal.Game.GameRowEffect[lethal.Game.Player2Index][2].RowStatus);

            var surviving = new HeadlessGameFixture();
            surviving.AddCard(surviving.Game.Player1Index, CardId.IgnisFatuus, RowPosition.MyRow1);
            var survivingPhantom = surviving.AddCard(
                surviving.Game.Player1Index, "70085", RowPosition.MyRow1);
            await surviving.Game.GameRowEffect[surviving.Game.Player2Index][1]
                .SetStatus<ImpenetrableFogStatus>();
            surviving.AddCard(
                surviving.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2, strength: 7);
            await surviving.SynchronizeClientsAsync();

            await surviving.Game.AddTask(async () =>
                await survivingPhantom.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowStatus.ImpenetrableFog, surviving.Game.GameRowEffect[surviving.Game.Player2Index][1].RowStatus);

            var noIgnis = new HeadlessGameFixture();
            var ordinaryPhantom = noIgnis.AddCard(
                noIgnis.Game.Player1Index, "70085", RowPosition.MyRow1);
            await noIgnis.Game.GameRowEffect[noIgnis.Game.Player2Index][1]
                .SetStatus<ImpenetrableFogStatus>();
            noIgnis.AddCard(
                noIgnis.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, strength: 6);
            await noIgnis.SynchronizeClientsAsync();

            await noIgnis.Game.AddTask(async () =>
                await ordinaryPhantom.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowStatus.BitingFrost, noIgnis.Game.GameRowEffect[noIgnis.Game.Player2Index][1].RowStatus);

            var ordinarySurvivor = new HeadlessGameFixture();
            var ordinarySurvivingPhantom = ordinarySurvivor.AddCard(
                ordinarySurvivor.Game.Player1Index, "70085", RowPosition.MyRow1);
            await ordinarySurvivor.Game.GameRowEffect[ordinarySurvivor.Game.Player2Index][1]
                .SetStatus<ImpenetrableFogStatus>();
            ordinarySurvivor.AddCard(
                ordinarySurvivor.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2, strength: 7);
            await ordinarySurvivor.SynchronizeClientsAsync();

            await ordinarySurvivor.Game.AddTask(async () =>
                await ordinarySurvivingPhantom.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Equal(RowStatus.ImpenetrableFog, ordinarySurvivor.Game.GameRowEffect[ordinarySurvivor.Game.Player2Index][1].RowStatus);
        }

        [Fact]
        public async Task ApiarianPhantomUsesSixDamageAndAppliesFrostOnlyOnKill()
        {
            var fixture = new HeadlessGameFixture();
            var phantom = fixture.AddCard(
                fixture.Game.Player1Index, "70085", RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, strength: 6);
            await fixture.SynchronizeClientsAsync();

            await phantom.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Equal(
                RowStatus.BitingFrost,
                fixture.Game.GameRowEffect[fixture.Game.Player2Index][1].RowStatus);
        }
    }
}
