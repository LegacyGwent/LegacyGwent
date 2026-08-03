using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class QuenLifecycleTests
    {
        [Fact]
        public async Task QuenImmediatelyBoostsAndShieldsSameIdCardsInHandAndDeck()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();
            var quen = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Quen,
                RowPosition.MyHand);
            var protectedCards = new[]
            {
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.IceTroll,
                    RowPosition.MyHand),
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.IceTroll,
                    RowPosition.MyHand),
                fixture.AddCard(
                    fixture.Game.Player1Index,
                    CardId.IceTroll,
                    RowPosition.MyDeck)
            };
            var unrelated = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await quen.Effect.CardUse();

            Assert.All(protectedCards, card =>
            {
                Assert.Equal(2, card.Status.HealthStatus);
                Assert.True(card.Status.IsShield);
            });
            Assert.Equal(0, unrelated.Status.HealthStatus);
            Assert.False(unrelated.Status.IsShield);
        }

        [Fact]
        public async Task QuenBlocksDamageToARevealedUnitInHand()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();
            var quen = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Quen,
                RowPosition.MyHand);
            var protectedUnit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyHand);
            protectedUnit.Status.IsReveal = true;
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await quen.Effect.CardUse();
            await protectedUnit.Effect.Damage(4, enemy);

            Assert.Equal(2, protectedUnit.Status.HealthStatus);
            Assert.False(protectedUnit.Status.IsShield);
            Assert.Equal(RowPosition.MyHand, protectedUnit.Status.CardRow);
        }

        [Fact]
        public async Task DuelInitiatorKeepsShieldUntilTheCounterattackConsumesIt()
        {
            var fixture = new HeadlessGameFixture();
            var duelist = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyRow1,
                strength: 6);
            var target = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 7);
            GiveQuen(duelist);
            await fixture.SynchronizeClientsAsync();

            await duelist.Effect.Duel(target, duelist);

            Assert.True(duelist.IsAliveOnPlance());
            Assert.Equal(0, duelist.Status.HealthStatus);
            Assert.False(duelist.Status.IsShield);
            Assert.Contains(target, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task TwoQuenUnitsResolveWithoutLooping()
        {
            var fixture = new HeadlessGameFixture();
            var first = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyRow1,
                strength: 4);
            var second = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 4);
            GiveQuen(first);
            GiveQuen(second);
            await fixture.SynchronizeClientsAsync();

            await first.Effect.Duel(second, first);

            Assert.True(first.IsAliveOnPlance());
            Assert.False(first.Status.IsShield);
            Assert.Contains(second, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task ForcedDuelStillTreatsTheFirstUnitAsTheInitiator()
        {
            var fixture = new HeadlessGameFixture();
            var first = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyRow1,
                strength: 4);
            var second = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 4);
            var treason = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.Treason,
                RowPosition.MyStay);
            GiveQuen(first);
            GiveQuen(second);
            await fixture.SynchronizeClientsAsync();

            await first.Effect.Duel(second, treason);

            Assert.True(first.IsAliveOnPlance());
            Assert.False(first.Status.IsShield);
            Assert.Contains(second, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
        }

        [Fact]
        public async Task ShieldOnTheDuelTargetStillBlocksTheFirstAttack()
        {
            var fixture = new HeadlessGameFixture();
            var duelist = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyRow1,
                strength: 4);
            var target = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 5);
            GiveQuen(target);
            await fixture.SynchronizeClientsAsync();

            await duelist.Effect.Duel(target, duelist);

            Assert.Contains(duelist, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.True(target.IsAliveOnPlance());
            Assert.False(target.Status.IsShield);
        }

        [Fact]
        public async Task IceTrollSelfDamageConsumesShieldBeforeItsFrostDuel()
        {
            var fixture = new HeadlessGameFixture();
            var iceTroll = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyHand,
                strength: 5);
            var target = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 6);
            await fixture.Game.GameRowEffect[fixture.Game.Player2Index][0]
                .SetStatus<BitingFrostStatus>();
            GiveQuen(iceTroll);
            await fixture.SynchronizeClientsAsync();

            await iceTroll.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(iceTroll.IsAliveOnPlance());
            Assert.False(iceTroll.Status.IsShield);
            Assert.Equal(0, iceTroll.Status.HealthStatus);
            Assert.Contains(target, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task IceTrollDamagesItselfBeforeStartingAnOrdinaryDuel()
        {
            var fixture = new HeadlessGameFixture();
            var iceTroll = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyHand,
                strength: 5);
            var target = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 4);
            await fixture.SynchronizeClientsAsync();

            await iceTroll.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(iceTroll.IsAliveOnPlance());
            Assert.Equal(-1, iceTroll.Status.HealthStatus);
            Assert.Contains(target, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        private static void GiveQuen(GameCard card)
        {
            card.Status.IsShield = true;
        }
    }
}
