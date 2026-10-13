using System.Linq;
using System.Threading.Tasks;
using Xunit;
namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberTwelfthBatchTests
    {
        [Theory]
        [InlineData(false, 0, 20, 9)]
        [InlineData(false, 4, 20, 5)]
        [InlineData(true, 0, 20, 0)]
        [InlineData(false, 0, 3, 3)]
        public async Task ArtisCountsActualPowerLost(bool shield, int armor, int strength, int expected)
        {
            var f = new HeadlessGameFixture();
            var ally = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2, strength);
            ally.Status.IsShield = shield; ally.Status.Armor = armor;
            var artis = f.AddCard(f.Game.Player1Index, CardId.Artis, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => artis.Effect.CardPlayEffect(false, false));
            var spawned = f.Game.GetPlaceCards(f.Game.Player1Index).Where(x => x.Status.CardId == CardId.SvalblodFanatic).ToList();
            if (expected == 0) Assert.Empty(spawned);
            else Assert.Equal(expected, Assert.Single(spawned).CardPoint());
            Assert.All(spawned, x => { Assert.True(x.Status.IsDoomed); Assert.Equal(RowPosition.MyRow1, x.Status.CardRow); });
        }

        [Theory]
        [InlineData("62011", false)]
        [InlineData("62011", true)]
        [InlineData("13025", false)]
        [InlineData("13025", true)]
        [InlineData("64013", false)]
        [InlineData("64013", true)]
        public async Task EveryCommonTransformPathClonesCultistBeforeSelfDamage(string sourceId, bool allied)
        {
            var f = new HeadlessGameFixture();
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            var cultist = f.AddCard(f.Game.Player1Index, CardId.CultistOblation, RowPosition.MyRow1);
            cultist.Status.Strength = 12; cultist.Status.HealthStatus = 3;
            cultist.Status.Armor = 1; cultist.Status.IsResilience = true;
            var target = f.AddCard(allied ? f.Game.Player1Index : f.Game.Player2Index, sourceId == "64013" ? sourceId : CardId.Wolf, RowPosition.MyRow2, 20);
            var source = sourceId == "64013" ? target : f.AddCard(f.Game.Player1Index, sourceId, RowPosition.MyStay);
            f.FirstPlayer.PlaceSelectionOverride = info => new[] { f.Game.GetCardLocation(source.PlayerIndex, target) };
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () =>
            {
                if (sourceId == "64013") await target.Effect.Damage(1, cultist);
                else if (sourceId == "13025") await source.Effect.CardUseEffect();
                else await source.Effect.CardPlayEffect(false, false);
            });
            var row = f.Game.RowToList(f.Game.Player1Index, RowPosition.MyRow1);
            Assert.Equal(2, row.Count);
            var copy = row[0];
            Assert.Same(cultist, row[1]); Assert.Equal(CardId.CultistOblation, copy.Status.CardId);
            Assert.True(copy.Status.IsDoomed); Assert.Equal(12, copy.Status.Strength);
            Assert.Equal(3, copy.Status.HealthStatus); Assert.Equal(1, copy.Status.Armor);
            Assert.True(copy.Status.IsResilience); Assert.Equal(0, cultist.Status.Armor);
            Assert.Equal(0, cultist.Status.HealthStatus);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task CultistIgnoresEnemyTurnsAndLock(bool enemyTurn, bool locked)
        {
            var f = new HeadlessGameFixture();
            f.Game.GameRound = (TwoPlayer)(enemyTurn ? f.Game.Player2Index : f.Game.Player1Index);
            var cultist = f.AddCard(f.Game.Player1Index, CardId.CultistOblation, RowPosition.MyRow1);
            cultist.Status.IsLock = locked;
            var target = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => target.Effect.Transform(CardId.RagingBear, target));
            Assert.Single(f.Game.RowToList(f.Game.Player1Index, RowPosition.MyRow1));
            Assert.Equal(9, cultist.CardPoint());
        }

        [Theory]
        [InlineData("43009", 1)]
        [InlineData("65004", -1)]
        public async Task CopiedDiyDeathwishAffectsTheOnlyRemainingUnit(string cardId, int change)
        {
            var f = new HeadlessGameFixture();
            var card = f.AddCard(f.Game.Player1Index, cardId, RowPosition.MyRow1);
            var survivor = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var elsewhere = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2, 20);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => card.Effect.Damage(100, survivor));
            Assert.Equal(change, survivor.Status.HealthStatus);
            Assert.Equal(0, elsewhere.Status.HealthStatus);
        }

        [Fact]
        public async Task VigoTransformingASpecialIntoAUnitDoesNotTriggerCultist()
        {
            var f = new HeadlessGameFixture();
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            var cultist = f.AddCard(f.Game.Player1Index, CardId.CultistOblation, RowPosition.MyRow1);
            var special = f.AddCard(f.Game.Player1Index, "13025", RowPosition.MyHand);
            var vigo = f.AddCard(f.Game.Player1Index, "70103", RowPosition.MyRow2);
            f.FirstPlayer.QueueMenuCardIds("13025");
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => vigo.Effect.CardPlayEffect(false, false));
            Assert.Equal(CardType.Unit, special.Status.Type);
            Assert.NotEqual("13025", special.Status.CardId);
            Assert.Single(f.Game.RowToList(f.Game.Player1Index, RowPosition.MyRow1));
            Assert.Equal(9, cultist.CardPoint());
        }

        [Fact]
        public async Task EngineerGrantsResilienceWithoutBoost()
        {
            var f = new HeadlessGameFixture();
            var ally = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var engineer = f.AddCard(f.Game.Player1Index, "34018", RowPosition.MyRow2);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => engineer.Effect.CardPlayEffect(false, false));
            Assert.True(ally.Status.IsResilience); Assert.Equal(20, ally.CardPoint());
        }

        [Fact]
        public async Task SkjordalCanDiscardGoldClanButOnlyResurrectDifferentBronze()
        {
            var f = new HeadlessGameFixture();
            var discarded = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            discarded.Status.Group = Group.Gold;
            discarded.Status.Categories = new[] { Categorie.ClanAnCraite };
            var same = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var wrongClan = f.AddCard(f.Game.Player1Index, CardId.Eskel, RowPosition.MyCemetery);
            wrongClan.Status.Group = Group.Copper;
            wrongClan.Status.Categories = new[] { Categorie.ClanDrummond };
            var resurrected = f.AddCard(f.Game.Player1Index, CardId.Lambert, RowPosition.MyCemetery);
            resurrected.Status.Group = Group.Copper;
            resurrected.Status.Categories = new[] { Categorie.ClanAnCraite };
            var skjordal = f.AddCard(f.Game.Player1Index, CardId.SkjordalDrummond, RowPosition.MyRow1);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => skjordal.Effect.CardPlayEffect(false, false));
            Assert.Equal(RowPosition.MyCemetery, discarded.Status.CardRow);
            Assert.Equal(RowPosition.MyStay, resurrected.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, same.Status.CardRow);
            Assert.Equal(RowPosition.MyCemetery, wrongClan.Status.CardRow);
        }

        [Fact]
        public async Task CultistDoesNotReactToForcedSameIdentityTransform()
        {
            var f = new HeadlessGameFixture();
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            var cultist = f.AddCard(f.Game.Player1Index, CardId.CultistOblation, RowPosition.MyRow1);
            var target = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2);
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(() => target.Effect.Transform(CardId.Wolf, target, isForce: true));
            Assert.Single(f.Game.RowToList(f.Game.Player1Index, RowPosition.MyRow1));
        }
    }
}
