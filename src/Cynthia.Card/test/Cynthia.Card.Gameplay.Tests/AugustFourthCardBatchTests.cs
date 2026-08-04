using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustFourthCardBatchTests
    {
        [Fact]
        public async Task LivingArmorLetsTheOriginalGreatswordSurviveAndTriggerItsEngine()
        {
            var fixture = new HeadlessGameFixture();
            fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.LivingArmor,
                RowPosition.MyRow1);
            var greatsword = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.AnCraiteGreatsword,
                RowPosition.MyRow1);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await greatsword.Effect.CardPlayEffect(false, false);
            await greatsword.Effect.Damage(9, enemy);

            Assert.True(greatsword.Status.CardRow.IsOnPlace());
            Assert.Equal(-5, greatsword.Status.HealthStatus);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));

            Assert.Equal(10, greatsword.Status.Strength);
            Assert.Equal(0, greatsword.Status.HealthStatus);
        }

        [Fact]
        public async Task CalantheConsumesOnlyPositiveBoostLeavesArmorAndIgnoresShieldThenReplaysTheUnit()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var spyingUnit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.RedanianKnight,
                RowPosition.MyRow1);
            spyingUnit.Status.IsSpying = true;
            var target = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.AnCraiteGreatsword,
                RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.LivingArmor,
                RowPosition.MyRow1);
            var calanthe = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.QueenCalanthe,
                RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();
            await target.Effect.Boost(5, target);
            await target.Effect.Armor(4, target);
            target.Status.IsShield = true;

            await calanthe.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));

            Assert.Equal(5, calanthe.Status.HealthStatus);
            Assert.Equal(0, calanthe.Status.Armor);
            Assert.Equal(0, target.Status.HealthStatus);
            Assert.Equal(4, target.Status.Armor);
            Assert.True(target.Status.IsShield);
            Assert.True(target.Status.CardRow.IsOnPlace());
            Assert.True(spyingUnit.Status.CardRow.IsOnPlace());
            Assert.True(spyingUnit.Status.IsSpying);
            Assert.Equal(1, fixture.FirstPlayer.LastMenuOptionCount);
        }

        [Fact]
        public async Task MeveBoostsOneUnitOnBoardInHandAndInDeckByFour()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var boardUnit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.RedanianKnight,
                RowPosition.MyRow1);
            var handUnit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.AnCraiteGreatsword,
                RowPosition.MyHand);
            var deckUnit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyDeck);
            var meve = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Meve,
                RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await meve.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));

            Assert.Equal(4, boardUnit.Status.HealthStatus);
            Assert.Equal(4, handUnit.Status.HealthStatus);
            Assert.Equal(4, deckUnit.Status.HealthStatus);
            Assert.Equal(0, meve.Status.HealthStatus);
        }

        [Fact]
        public async Task AnnaSortsTheDeckAndPlaysTheLowestBaseStrengthCard()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var high = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyDeck,
                strength: 10);
            var low = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.RedanianKnight,
                RowPosition.MyDeck,
                strength: 3);
            var middle = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.AnCraiteGreatsword,
                RowPosition.MyDeck,
                strength: 7);
            var anna = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.AnnaHenrietta,
                RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await anna.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(low.Status.CardRow.IsOnPlace());
            Assert.Equal(new[] { middle, high }, fixture.Game.PlayersDeck[
                fixture.Game.Player1Index]);
        }

        [Fact]
        public async Task DanaPlayingANeutralGoldFromDeckSummonsRoach()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var gold = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyDeck);
            var roach = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Roach,
                RowPosition.MyDeck);
            var dana = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.DanaMeadbh,
                RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await dana.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(gold.Status.CardRow.IsOnPlace());
            Assert.True(roach.Status.CardRow.IsOnPlace());
            Assert.Equal(2, fixture.FirstPlayer.LastMenuOptionCount);
        }

        [Fact]
        public async Task DanaCanChainRoyalDecreeIntoAGoldUnitAndSummonRoach()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var decree = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.RoyalDecree,
                RowPosition.MyDeck);
            var gold = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyDeck);
            var roach = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Roach,
                RowPosition.MyDeck);
            var dana = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.DanaMeadbh,
                RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await dana.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(gold.Status.CardRow.IsOnPlace());
            Assert.Equal(2, gold.Status.HealthStatus);
            Assert.True(decree.Status.CardRow.IsInCemetery());
            Assert.True(roach.Status.CardRow.IsOnPlace());
        }

        [Fact]
        public async Task SummoningAGoldUnitDoesNotTriggerRoach()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var gold = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyDeck);
            var roach = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Roach,
                RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await gold.Effect.Summon(new CardLocation(RowPosition.MyRow1, 0), gold);

            Assert.True(gold.Status.CardRow.IsOnPlace());
            Assert.Contains(roach, fixture.Game.PlayersDeck[fixture.Game.Player1Index]);
        }
    }
}
