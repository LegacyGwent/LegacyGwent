using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class AugustThirdCardBatchTests
    {
        [Fact]
        public async Task LivingArmorHalvesDamageOnceForTheWholeRowIncludingItself()
        {
            var fixture = new HeadlessGameFixture();
            var firstArmor = fixture.AddCard(
                fixture.Game.Player1Index, CardId.LivingArmor, RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.LivingArmor, RowPosition.MyRow1);
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1, strength: 20);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await firstArmor.Effect.CardPlayEffect(false, false);
            await ally.Effect.Damage(9, enemy);
            await firstArmor.Effect.Damage(9, enemy);

            Assert.Equal(0, firstArmor.Status.Armor);
            Assert.Equal(-5, ally.Status.HealthStatus);
            Assert.Equal(-5, firstArmor.Status.HealthStatus);
        }

        [Fact]
        public async Task IvoDeathwishTriggersOnlyWhenIvoDies()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var ivo = fixture.AddCard(
                fixture.Game.Player1Index, "70026", RowPosition.MyRow1);
            var other = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow2);
            var nonWitcher = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Roach, RowPosition.MyDeck);
            var witcher = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Eskel, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await ivo.Effects.RaiseEvent(new AfterCardDeath(other, other.GetLocation(other.PlayerIndex)));
            Assert.Same(nonWitcher, fixture.Game.PlayersDeck[fixture.Game.Player1Index].First());

            await ivo.Effects.RaiseEvent(new AfterCardDeath(ivo, ivo.GetLocation(ivo.PlayerIndex)));
            Assert.Same(witcher, fixture.Game.PlayersDeck[fixture.Game.Player1Index].First());
        }

        [Fact]
        public async Task LadyOfTheLakeWeakensByTheCardCountWithoutDoubling()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();
            var lady = fixture.AddCard(
                fixture.Game.Player1Index, CardId.LadyOfTheLake, RowPosition.MyHand);
            for (var i = 0; i < 3; i++)
            {
                fixture.AddCard(
                    fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            }
            for (var i = 0; i < 2; i++)
            {
                fixture.AddCard(
                    fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            }
            await fixture.SynchronizeClientsAsync();

            await lady.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(17, lady.Status.Strength);
            Assert.Equal(0, lady.Status.HealthStatus);
        }

        [Fact]
        public async Task ThawAppliesFourBaseBoostsWhenNoEarlierCardWasPlayed()
        {
            var fixture = new HeadlessGameFixture();
            var thaw = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Thaw, RowPosition.MyHand);
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await thaw.Effect.CardUse();

            Assert.Equal(8, ally.Status.HealthStatus);
        }

        [Fact]
        public async Task LonelyChampionUsesRowAndBoardBonusesIndependently()
        {
            var fixture = new HeadlessGameFixture();
            var champion = fixture.AddCard(
                fixture.Game.Player1Index, CardId.LonelyChampion, RowPosition.MyRow1);
            var ally = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await champion.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(0, champion.Status.HealthStatus);

            await ally.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), ally);
            await champion.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(1, champion.Status.HealthStatus);

            await ally.Effect.ToCemetery();
            await champion.Effects.RaiseEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(5, champion.Status.HealthStatus);
        }

        [Fact]
        public async Task PrincessTransformsOneBearAtEachOwnerTurnStart()
        {
            var fixture = new HeadlessGameFixture();
            var princess = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Princess, RowPosition.MyRow1);
            var bears = new[]
            {
                fixture.AddCard(fixture.Game.Player1Index, "15010", RowPosition.MyRow1),
                fixture.AddCard(fixture.Game.Player1Index, "15010", RowPosition.MyRow1)
            };
            await fixture.SynchronizeClientsAsync();

            await princess.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.All(bears, bear => Assert.Equal("15010", bear.Status.CardId));

            await princess.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Single(bears, bear => bear.Status.CardId == CardId.RagingBear);

            await princess.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.All(bears, bear => Assert.Equal(CardId.RagingBear, bear.Status.CardId));
        }

        [Fact]
        public async Task OldSpeartipCompletesBothTransformEffects()
        {
            var fixture = new HeadlessGameFixture();
            var speartip = fixture.AddCard(
                fixture.Game.Player1Index, CardId.OldSpeartipAsleep, RowPosition.MyRow1);
            var adjacent = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var enemies = Enumerable.Range(0, 3)
                .Select(_ => fixture.AddCard(
                    fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1))
                .ToArray();
            await fixture.SynchronizeClientsAsync();

            await speartip.Effect.CardPlayEffect(false, false);
            Assert.Equal(5, speartip.Status.Armor);
            Assert.Equal(1, adjacent.Status.HealthStatus);

            await speartip.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(CardId.OldSpeartip, speartip.Status.CardId);
            Assert.IsType<OldSpeartip>(speartip.Effect);
            Assert.All(enemies, enemy => Assert.Equal(-2, enemy.Status.HealthStatus));

            await enemies[0].Effect.ToCemetery();
            await speartip.Effects.RaiseEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(CardId.OldSpeartipAsleep, speartip.Status.CardId);
            Assert.IsType<OldSpeartipAsleep>(speartip.Effect);
            Assert.Equal(5, speartip.Status.Armor);
            Assert.Equal(2, adjacent.Status.HealthStatus);
        }

        [Fact]
        public async Task AguaraRandomHandBoostExcludesSpies()
        {
            var fixture = new HeadlessGameFixture();
            var aguara = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Aguara, RowPosition.MyRow1);
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var spy = fixture.AddCard(
                fixture.Game.Player1Index, CardId.Emissary, RowPosition.MyHand);
            var loyal = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await aguara.Effect.CardPlayEffect(false, false);

            Assert.Equal(0, spy.Status.HealthStatus);
            Assert.Equal(5, loyal.Status.HealthStatus);
        }

        [Fact]
        public async Task MagicLampInCemeteryMakesLastWishInspectOneMoreCard()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.AddCard(
                fixture.Game.Player1Index, CardId.MagicLamp, RowPosition.MyCemetery);
            var lastWish = fixture.AddCard(
                fixture.Game.Player1Index, CardId.TheLastWish, RowPosition.MyStay);
            for (var i = 0; i < 5; i++)
            {
                fixture.AddCard(
                    fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            }
            await fixture.SynchronizeClientsAsync();

            await lastWish.Effect.CardUseEffect();

            Assert.Equal(3, fixture.FirstPlayer.LastMenuOptionCount);
        }
    }
}
