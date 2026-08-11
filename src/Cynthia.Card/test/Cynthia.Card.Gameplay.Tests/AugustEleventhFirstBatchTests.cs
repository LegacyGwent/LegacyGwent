using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustEleventhFirstBatchTests
    {
        [Fact]
        public async Task RumourmongerCopiesTopmostEnemyCopperThenBothPlayersDraw()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();
            var rumourmonger = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Rumourmonger, RowPosition.MyRow1);
            var ownDraw = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var enemyGold = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var enemyCopper = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await rumourmonger.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Contains(ownDraw, fixture.Game.PlayersHandCard[fixture.Game.Player1Index]);
            var drawnCopy = Assert.Single(
                fixture.Game.PlayersHandCard[fixture.Game.Player2Index],
                card => card.Status.CardId == enemyCopper.Status.CardId);
            Assert.NotSame(enemyCopper, drawnCopy);
            Assert.Contains(enemyGold, fixture.Game.PlayersDeck[fixture.Game.Player2Index]);
            Assert.Contains(enemyCopper, fixture.Game.PlayersDeck[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task RumourmongerDoesNothingWhenEnemyDeckHasNoCopper()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();
            var rumourmonger = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Rumourmonger, RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await rumourmonger.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Empty(fixture.Game.PlayersHandCard[fixture.Game.Player1Index]);
            Assert.Empty(fixture.Game.PlayersHandCard[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task HeftyHelgeRevealCounterRepeatsOffRowDamageThenResetsAcrossResurrection()
        {
            var fixture = new HeadlessGameFixture();
            var helge = fixture.AddCard(
                fixture.Game.Player1Index, CardId.HeftyHelge, RowPosition.MyHand);
            var allySource = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow3);
            var enemySource = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow3);
            var sameRowEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var otherRowEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 20);
            await fixture.SynchronizeClientsAsync();

            Assert.Equal(1, helge.Status.Countdown);
            await fixture.Game.SendEvent(new AfterCardReveal(helge, enemySource));
            Assert.Equal(1, helge.Status.Countdown);
            await fixture.Game.SendEvent(new AfterCardReveal(helge, allySource));
            await fixture.Game.SendEvent(new AfterCardReveal(helge, allySource));
            Assert.Equal(3, helge.Status.Countdown);

            await fixture.Game.AddTask(async () => await helge.Effect.Play(
                new CardLocation(RowPosition.MyRow1, 0), false, true));

            Assert.Equal(0, sameRowEnemy.Status.HealthStatus);
            Assert.Equal(-3, otherRowEnemy.Status.HealthStatus);
            Assert.Equal(0, helge.Status.Countdown);

            await helge.Effect.ToCemetery();
            await helge.Effect.Resurrect(new CardLocation(RowPosition.MyRow1, 0), helge);
            Assert.Equal(0, helge.Status.Countdown);
        }

        [Fact]
        public async Task CongregationClericCopiesOnlyLockedCopperUnitsAtLeastTwoPower()
        {
            var fixture = new HeadlessGameFixture();
            var cleric = fixture.AddCard(
                fixture.Game.Player1Index, CardId.CongregationCleric, RowPosition.MyRow1);
            var tooWeak = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow2, 1);
            tooWeak.Status.IsLock = true;
            var eligible = fixture.AddCard(
                fixture.Game.Player2Index, CardId.ArachasHatchling, RowPosition.MyRow2, 2);
            eligible.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await cleric.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.DoesNotContain(
                fixture.Game.PlayersPlace[fixture.Game.Player1Index][0],
                card => card != cleric && card.Status.CardId == tooWeak.Status.CardId);
            var copy = Assert.Single(
                fixture.Game.PlayersPlace[fixture.Game.Player1Index][0],
                card => card.Status.CardId == eligible.Status.CardId);
            Assert.Equal(2, copy.Status.Strength);
            Assert.True(copy.Status.IsDoomed);
        }

        [Fact]
        public async Task ArnjolfDestroysAllLowPowerAlliesBeforeAllLowPowerEnemies()
        {
            var fixture = new HeadlessGameFixture();
            var arnjolf = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ArnjolfThePatricide, RowPosition.MyRow1);
            var lowAlly = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow2, 2);
            lowAlly.Status.IsDoomed = false;
            var highAlly = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyRow2, 3);
            var lowEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2, 2);
            lowEnemy.Status.IsDoomed = false;
            var highEnemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.Eskel, RowPosition.MyRow2, 3);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(async () =>
                await arnjolf.Effects.RaiseEvent(new CardPlayEffect(false, false)));

            Assert.Contains(lowAlly, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.Contains(lowEnemy, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
            Assert.True(highAlly.Status.CardRow.IsOnPlace());
            Assert.True(highEnemy.Status.CardRow.IsOnPlace());
            Assert.True(arnjolf.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task SvalblodExcludesSpiesFromBothHandAndDeckRules()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var svalblod = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Svalblod, RowPosition.MyRow1);
            var deckSpy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck, 4);
            deckSpy.Status.IsSpying = true;
            var handSpy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand, 4);
            handSpy.Status.IsSpying = true;
            var normal = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck, 4);
            await fixture.SynchronizeClientsAsync();

            await svalblod.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.Equal(4, deckSpy.Status.Strength);
            Assert.Equal(0, deckSpy.Status.HealthStatus);
            Assert.Equal(4, handSpy.Status.Strength);
            Assert.Equal(0, handSpy.Status.HealthStatus);
            Assert.Equal(6, normal.Status.Strength);
            Assert.Equal(-2, normal.Status.HealthStatus);
        }

        [Fact]
        public async Task SigvaldResurrectsAndStrengthensOnlyAfterTwoOwnerTurnEnds()
        {
            var fixture = new HeadlessGameFixture();
            var sigvald = fixture.AddCard(
                fixture.Game.Player1Index, "70038", RowPosition.MyCemetery);
            var originalStrength = sigvald.Status.Strength;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            Assert.True(sigvald.Status.CardRow.IsInCemetery());

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.True(sigvald.Status.CardRow.IsOnPlace());
            Assert.Equal(originalStrength + 1, sigvald.Status.Strength);
        }

        [Fact]
        public async Task CrowmotherIsNotDoomedAsAnIntrinsicCardProperty()
        {
            var fixture = new HeadlessGameFixture();
            var crowmother = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Crowmother, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            Assert.False(crowmother.Status.IsDoomed);
        }

        [Fact]
        public async Task SigrdrifaResurrectsAnySkelligeBronzeOrSilverUnitWithoutClanRequirement()
        {
            var fixture = new HeadlessGameFixture();
            var sigrdrifa = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Sigrdrifa, RowPosition.MyRow1);
            var neutral = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            neutral.Status.Group = Group.Copper;
            neutral.Status.Faction = Faction.Neutral;
            var nonClanSkellige = fixture.AddCard(
                fixture.Game.Player1Index, CardId.ArachasHatchling, RowPosition.MyCemetery);
            nonClanSkellige.Status.Group = Group.Copper;
            nonClanSkellige.Status.Faction = Faction.Skellige;
            nonClanSkellige.Status.Categories = new[] { Categorie.Beast };
            await fixture.SynchronizeClientsAsync();

            await sigrdrifa.Effects.RaiseEvent(new CardPlayEffect(false, false));

            Assert.True(nonClanSkellige.Status.CardRow.IsOnStay());
            Assert.True(neutral.Status.CardRow.IsInCemetery());
        }
    }
}
