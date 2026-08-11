using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustEighthFirstBatchTests
    {
        [Fact]
        public async Task RoachIgnoresLeadersAndSummonsForNonLeaderGoldUnits()
        {
            var fixture = new HeadlessGameFixture();
            var roach = fixture.AddCard(fixture.Game.Player1Index, CardId.Roach, RowPosition.MyDeck);
            var leader = fixture.AddCard(fixture.Game.Player1Index, "21001", RowPosition.MyRow1);
            var gold = fixture.AddCard(fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterUnitDown(
                leader, true, false, (false, false), false, true));
            Assert.True(roach.Status.CardRow.IsInDeck());

            await fixture.Game.SendEvent(new AfterUnitDown(
                gold, true, false, (false, false), false, true));
            Assert.True(roach.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task WraithCycleRunsAtEveryTurnEndAndPreservesPower()
        {
            var fixture = new HeadlessGameFixture();
            var noon = fixture.AddCard(fixture.Game.Player1Index, "70185", RowPosition.MyRow1);
            noon.Status.HealthStatus = 3;
            await fixture.SynchronizeClientsAsync();

            await noon.Effect.CardPlayEffect(false, false);
            Assert.Single(fixture.Game.PlayersPlace[fixture.Game.Player1Index][0],
                card => card.Status.CardId == "70186");
            Assert.Single(fixture.Game.PlayersPlace[fixture.Game.Player2Index][0],
                card => card.Status.CardId == "70186");

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            Assert.Equal("70187", noon.Status.CardId);
            Assert.Equal(10, noon.CardPoint());
            Assert.Equal(2, fixture.Game.PlayersPlace[fixture.Game.Player1Index][0]
                .Count(card => card.Status.CardId == "70186"));
            Assert.Equal(2, fixture.Game.PlayersPlace[fixture.Game.Player2Index][0]
                .Count(card => card.Status.CardId == "70186"));

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal("70185", noon.Status.CardId);
            Assert.Equal(10, noon.CardPoint());
            Assert.All(
                fixture.Game.PlayersPlace.SelectMany(rows => rows)
                    .SelectMany(row => row)
                    .Where(card => card.Status.CardId == "70186"),
                mirror => Assert.Equal(2, mirror.CardPoint()));
        }

        [Fact]
        public async Task EskelCanDestroyABoostedLockedEnemy()
        {
            var fixture = new HeadlessGameFixture();
            var eskel = fixture.AddCard(fixture.Game.Player1Index, "12016", RowPosition.MyRow1);
            var target = fixture.AddCard(fixture.Game.Player2Index, "44020", RowPosition.MyRow1, 20);
            target.Status.HealthStatus = 5;
            target.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await eskel.Effect.CardPlayEffect(false, false);

            Assert.Contains(target, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task GeraltProfessionalDestroysMonsterCategoriesButOnlyDamagesOrdinaryUnits()
        {
            var destroyFixture = new HeadlessGameFixture();
            var geralt = destroyFixture.AddCard(
                destroyFixture.Game.Player1Index, "12017", RowPosition.MyRow1);
            var beast = destroyFixture.AddCard(
                destroyFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            beast.Status.Faction = Faction.Neutral;
            beast.Status.Categories = new[] { Categorie.Beast };
            await destroyFixture.SynchronizeClientsAsync();

            await geralt.Effect.CardPlayEffect(false, false);
            Assert.Contains(beast, destroyFixture.Game.PlayersCemetery[destroyFixture.Game.Player2Index]);

            var damageFixture = new HeadlessGameFixture();
            var secondGeralt = damageFixture.AddCard(
                damageFixture.Game.Player1Index, "12017", RowPosition.MyRow1);
            var soldier = damageFixture.AddCard(
                damageFixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            soldier.Status.Faction = Faction.Neutral;
            soldier.Status.Categories = new[] { Categorie.Soldier };
            await damageFixture.SynchronizeClientsAsync();

            await secondGeralt.Effect.CardPlayEffect(false, false);
            Assert.Equal(-4, soldier.Status.HealthStatus);
            Assert.True(soldier.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task SporesPiercesArmorAndConsumesBoonForThreeDamage()
        {
            var fixture = new HeadlessGameFixture();
            var spores = fixture.AddCard(fixture.Game.Player1Index, "14025", RowPosition.MyHand);
            var enemies = new[]
            {
                fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20),
                fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 20),
                fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow3, 20)
            };
            foreach (var enemy in enemies)
            {
                enemy.Status.Armor = 5;
            }
            foreach (var row in fixture.Game.GameRowEffect[fixture.Game.Player2Index])
            {
                await row.SetStatus<GoldenFrothStatus>();
            }
            await fixture.SynchronizeClientsAsync();

            await spores.Effect.CardUseEffect();

            Assert.Equal(-3, enemies.Sum(enemy => enemy.Status.HealthStatus));
            Assert.All(enemies, enemy => Assert.Equal(5, enemy.Status.Armor));
            Assert.Single(fixture.Game.GameRowEffect[fixture.Game.Player2Index],
                row => row.RowStatus == RowStatus.None);
        }

        [Fact]
        public async Task PitTrapDealsArmorPiercingDamageOnContact()
        {
            var fixture = new HeadlessGameFixture();
            var target = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            target.Status.Armor = 5;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][0]
                .SetStatus<PitTrapStatus>();

            Assert.Equal(-3, target.Status.HealthStatus);
            Assert.Equal(5, target.Status.Armor);
        }

        [Fact]
        public async Task TridamStartsUnlockedWithFourArmorAndMaulerUsesLockedDamageValue()
        {
            var fixture = new HeadlessGameFixture();
            var tridam = fixture.AddCard(fixture.Game.Player1Index, "44001", RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();
            await fixture.Game.SendEvent(new OnGameStart());
            Assert.False(tridam.Status.IsLock);

            await tridam.Effect.CardPlayEffect(false, false);
            Assert.Equal(4, tridam.Status.Armor);

            var mauler = fixture.AddCard(fixture.Game.Player1Index, "44016", RowPosition.MyRow1);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            enemy.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();
            await mauler.Effect.CardPlayEffect(false, false);
            Assert.Equal(-7, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task ImmortalCavalryReactsOnlyToLockedUnits()
        {
            var fixture = new HeadlessGameFixture();
            var cavalry = fixture.AddCard(fixture.Game.Player1Index, "70101", RowPosition.MyRow1);
            var locked = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            var unlocked = fixture.AddCard(fixture.Game.Player1Index, CardId.RedanianKnight, RowPosition.MyRow1);
            locked.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await locked.Effect.Boost(2, cavalry);
            await locked.Effect.Damage(1, cavalry);
            await unlocked.Effect.Boost(2, cavalry);
            await unlocked.Effect.Damage(1, cavalry);

            Assert.Equal(2, cavalry.Status.HealthStatus);
        }

        [Fact]
        public async Task IorvethMeditationBoostsByActualDuelDamageInstances()
        {
            var fixture = new HeadlessGameFixture();
            var iorveth = fixture.AddCard(fixture.Game.Player1Index, "52012", RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 2);
            fixture.AddCard(fixture.Game.Player2Index, CardId.RedanianKnight, RowPosition.MyRow1, 3);
            await fixture.SynchronizeClientsAsync();

            await iorveth.Effect.CardPlayEffect(false, false);

            Assert.Equal(2, iorveth.Status.HealthStatus);
        }

        [Fact]
        public async Task VriheddSaboteurObservesEveryRealReturnToDeck()
        {
            var fixture = new HeadlessGameFixture();
            var saboteur = fixture.AddCard(fixture.Game.Player1Index, "70098", RowPosition.MyRow1);
            var card = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.LogicCardMove(card, fixture.Game.PlayersHandCard[fixture.Game.Player1Index], 0);
            Assert.Equal(0, saboteur.Status.HealthStatus);

            await fixture.Game.LogicCardMove(card, fixture.Game.PlayersDeck[fixture.Game.Player1Index], 0);
            Assert.Equal(1, saboteur.Status.HealthStatus);

            await fixture.Game.LogicCardMove(saboteur, fixture.Game.PlayersDeck[fixture.Game.Player1Index], 0);
            Assert.Equal(1, saboteur.Status.HealthStatus);
        }

        [Fact]
        public async Task BowDryadRepeatsOnlyWhenMovedToRangedDuringOwnerTurn()
        {
            var fixture = new HeadlessGameFixture();
            var dryad = fixture.AddCard(fixture.Game.Player1Index, "70114", RowPosition.MyRow1);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await dryad.Effect.CardPlayEffect(false, false);
            Assert.Equal(-4, enemy.Status.HealthStatus);

            fixture.Game.GameRound = (TwoPlayer)fixture.Game.Player1Index;
            await dryad.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad);
            Assert.Equal(-8, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task DwarfMinerCountsBoardHandDeckAndSelfCopiesFromEightBasePower()
        {
            var fixture = new HeadlessGameFixture();
            var miner = fixture.AddCard(fixture.Game.Player1Index, "70097", RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player1Index, "70097", RowPosition.MyRow2);
            fixture.AddCard(fixture.Game.Player1Index, "70097", RowPosition.MyHand);
            fixture.AddCard(fixture.Game.Player1Index, "70097", RowPosition.MyDeck);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await miner.Effect.CardPlayEffect(false, false);

            Assert.Equal(12, miner.Status.Strength);
        }

        [Fact]
        public async Task ForestWhispererSummonsForTwoUnrevealedAmbushes()
        {
            var fixture = new HeadlessGameFixture();
            var whisperer = fixture.AddCard(fixture.Game.Player1Index, "70100", RowPosition.MyDeck);
            var first = fixture.AddCard(fixture.Game.Player1Index, "53017", RowPosition.MyRow1);
            var second = fixture.AddCard(fixture.Game.Player1Index, "53018", RowPosition.MyRow2);
            first.Status.Conceal = true;
            second.Status.Conceal = true;
            first.Status.Categories = first.Status.Categories.Concat(new[] { Categorie.Ambush }).Distinct().ToArray();
            second.Status.Categories = second.Status.Categories.Concat(new[] { Categorie.Ambush }).Distinct().ToArray();
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterUnitDown(
                second, true, false, (false, false), false, true));

            Assert.True(whisperer.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task DryadMatronStrengthensOtherTreantsAndBoostsHandPerTreant()
        {
            var fixture = new HeadlessGameFixture();
            var matron = fixture.AddCard(fixture.Game.Player1Index, "70122", RowPosition.MyRow1);
            var firstTreant = fixture.AddCard(fixture.Game.Player1Index, "70117", RowPosition.MyRow1);
            var secondTreant = fixture.AddCard(fixture.Game.Player1Index, "70140", RowPosition.MyRow1);
            var handUnit = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await matron.Effect.CardPlayEffect(false, false);

            Assert.Equal(4, firstTreant.Status.Strength);
            Assert.Equal(7, secondTreant.Status.Strength);
            Assert.Equal(2, handUnit.Status.HealthStatus);
            Assert.Equal(6, matron.Status.Strength);
        }
    }
}
