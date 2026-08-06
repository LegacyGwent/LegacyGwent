using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustFifthMonsterBatchTests
    {
        [Fact]
        public async Task GeraltAardDealsTwoExtraDamageToSiegeTargetsBeforeTheBaseDamage()
        {
            var fixture = new HeadlessGameFixture();
            var aard = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltAard, RowPosition.MyRow1);
            var target = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow3, strength: 20);
            await fixture.SynchronizeClientsAsync();

            await aard.Effect.CardPlayEffect(false, false);

            Assert.Equal(-5, target.Status.HealthStatus);
            Assert.Equal(RowPosition.MyRow3, target.Status.CardRow);
        }

        [Fact]
        public async Task GeraltProfessionalBanishesAMonsterWithoutTriggeringDeathwish()
        {
            var fixture = new HeadlessGameFixture();
            var geralt = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltProfessional, RowPosition.MyRow1);
            var egg = fixture.AddCard(
                fixture.Game.Player2Index, CardId.HarpyEgg, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await geralt.Effect.CardPlayEffect(false, false);

            Assert.Equal(RowPosition.Banish, egg.Status.CardRow);
            Assert.DoesNotContain(
                fixture.Game.GetPlaceCards(fixture.Game.Player2Index),
                card => card.Status.CardId == CardId.HarpyHatchling);
        }

        [Fact]
        public async Task OlgierdVonEverecReturnsAtRoundStartAndLosesHalfHisBaseStrength()
        {
            var fixture = new HeadlessGameFixture();
            var olgierd = fixture.AddCard(
                fixture.Game.Player1Index, CardId.OlgierdVonEverec, RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            await olgierd.Effects.RaiseEvent(new AfterCardToCemetery(
                olgierd,
                new CardLocation(RowPosition.MyRow2, 0),
                true));
            await olgierd.Effects.RaiseEvent(new BeforeRoundStart(1));

            Assert.Equal(RowPosition.MyRow2, olgierd.Status.CardRow);
            Assert.Equal(4, olgierd.Status.Strength);
        }

        [Fact]
        public async Task ImlerithDestroysInsteadOfDamagingAUnitUnderBitingFrost()
        {
            var fixture = new HeadlessGameFixture();
            var imlerith = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Imlerith, RowPosition.MyRow1);
            var target = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2, strength: 20);
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][1]
                .SetStatus<BitingFrostStatus>();
            await fixture.SynchronizeClientsAsync();

            await imlerith.Effect.CardPlayEffect(false, false);

            Assert.Contains(target, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task DettlaffCanBanishGoldAndLeaderBeastsOrVampires()
        {
            var fixture = new HeadlessGameFixture();
            var dettlaff = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DetlaffCrimsonCurse, RowPosition.MyRow1);
            var gold = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var leader = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var silver = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            gold.Status.Group = Group.Gold;
            leader.Status.Group = Group.Leader;
            leader.Status.Categories = new[] { Categorie.Vampire };
            silver.Status.Group = Group.Silver;
            await fixture.SynchronizeClientsAsync();

            await dettlaff.Effect.CardPlayEffect(false, false);

            Assert.All(new[] { gold, leader, silver }, card =>
                Assert.Equal(RowPosition.Banish, card.Status.CardRow));
        }

        [Fact]
        public async Task SirScratchALotBoostsEachCopyOnceWhenAnAlliedBeastEntersPlay()
        {
            var fixture = new HeadlessGameFixture();
            var boardCopy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.SirScratchALot, RowPosition.MyRow1);
            var handCopy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.SirScratchALot, RowPosition.MyHand);
            var deckCopy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.SirScratchALot, RowPosition.MyDeck);
            var enemyBeast = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow3);
            await fixture.Game.GameRowEffect[fixture.Game.Player1Index][0]
                .SetStatus<FullMoonStatus>();
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][2]
                .SetStatus<FullMoonStatus>();
            await fixture.SynchronizeClientsAsync();

            await boardCopy.Effect.CardPlayEffect(false, false);
            await fixture.Game.CreateCardAtEnd(
                CardId.Wolf, fixture.Game.Player1Index, RowPosition.MyRow2);

            Assert.Equal(2, boardCopy.Status.HealthStatus);
            Assert.Equal(1, handCopy.Status.HealthStatus);
            Assert.Equal(1, deckCopy.Status.HealthStatus);
            Assert.Equal(1, enemyBeast.Status.HealthStatus);
        }

        [Fact]
        public async Task CloudGiantConsumesOneImmunityTurnPerOpponentFogRow()
        {
            var fixture = new HeadlessGameFixture();
            var giant = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CloudGiant, RowPosition.MyRow1);
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][0]
                .SetStatus<ImpenetrableFogStatus>();
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][2]
                .SetStatus<ImpenetrableFogStatus>();
            await fixture.SynchronizeClientsAsync();

            await giant.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.True(giant.Status.IsImmue);
            Assert.False(giant.Status.IsResilience);

            await giant.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(giant.Status.IsImmue);

            await giant.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.False(giant.Status.IsImmue);
        }

        [Fact]
        public async Task KeltullisRequiresAnAlliedSacrificeBeforeItsFollowUpEffects()
        {
            var fixture = new HeadlessGameFixture();
            var keltullis = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Keltullis, RowPosition.MyRow1);
            var sacrifice = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, strength: 2);
            var strongerAlly = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 8);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 5);
            await fixture.SynchronizeClientsAsync();
            await keltullis.Effect.CardPlayEffect(false, false);

            await keltullis.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.False(sacrifice.Status.CardRow.IsOnPlace());
            Assert.Contains(enemy, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
            Assert.Equal(1, keltullis.Status.HealthStatus);
            Assert.Equal(RowPosition.MyRow1, strongerAlly.Status.CardRow);

            var fixtureWithoutSacrifice = new HeadlessGameFixture();
            var loneKeltullis = fixtureWithoutSacrifice.AddCard(
                fixtureWithoutSacrifice.Game.Player1Index, CardId.Keltullis, RowPosition.MyRow1);
            var untouchedEnemy = fixtureWithoutSacrifice.AddCard(
                fixtureWithoutSacrifice.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixtureWithoutSacrifice.SynchronizeClientsAsync();

            await loneKeltullis.Effects.RaiseEvent(
                new AfterTurnOver(fixtureWithoutSacrifice.Game.Player1Index));

            Assert.Equal(0, loneKeltullis.Status.HealthStatus);
            Assert.Equal(RowPosition.MyRow1, untouchedEnemy.Status.CardRow);
        }

        [Fact]
        public async Task TatterwingMovesAndDamagesEveryEnemyOnTheOppositeRow()
        {
            var fixture = new HeadlessGameFixture();
            var tatterwing = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Tatterwing, RowPosition.MyRow1);
            var enemies = new[]
            {
                fixture.AddCard(fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1),
                fixture.AddCard(fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyRow1)
            };
            await fixture.SynchronizeClientsAsync();

            await tatterwing.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.All(enemies, enemy =>
            {
                Assert.NotEqual(RowPosition.MyRow1, enemy.Status.CardRow);
                Assert.Equal(-1, enemy.Status.HealthStatus);
            });
        }
    }
}
