using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class QuenLifecycleTests
    {
        [Fact]
        public async Task QuenMarksTheSelectedUnitAndSameIdCardsInHandAndDeck()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.Game.PlayersDeck[fixture.Game.Player2Index].Clear();
            var quen = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.Quen,
                RowPosition.MyHand);
            var markedCards = new[]
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

            Assert.All(markedCards, card => Assert.Single(
                card.Effects.OfType<PendingQuenEffect>(),
                effect => effect.IsPending));
            Assert.Empty(unrelated.Effects.OfType<PendingQuenEffect>());
        }

        [Fact]
        public async Task IceTrollGainsQuenAfterItsDeployDuelFinishes()
        {
            var fixture = new HeadlessGameFixture();
            var iceTroll = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyHand);
            var duelTarget = fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 1);
            var pendingQuen = new PendingQuenEffect(iceTroll);
            iceTroll.Effects.Add(pendingQuen);
            await fixture.SynchronizeClientsAsync();

            await iceTroll.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(iceTroll.IsAliveOnPlance());
            Assert.Equal(2, iceTroll.Status.HealthStatus);
            Assert.True(iceTroll.Status.IsShield);
            Assert.False(pendingQuen.IsPending);
            Assert.Contains(duelTarget, fixture.Game.PlayersCemetery[fixture.Game.Player2Index]);
        }

        [Fact]
        public async Task LethalDeployDuelDoesNotConsumePendingQuen()
        {
            var fixture = new HeadlessGameFixture();
            var iceTroll = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyHand);
            fixture.AddCard(
                fixture.Game.Player2Index,
                CardId.GeraltOfRivia,
                RowPosition.MyRow1,
                strength: 10);
            var pendingQuen = new PendingQuenEffect(iceTroll);
            iceTroll.Effects.Add(pendingQuen);
            await fixture.SynchronizeClientsAsync();

            await iceTroll.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Contains(iceTroll, fixture.Game.PlayersCemetery[fixture.Game.Player1Index]);
            Assert.False(iceTroll.Status.IsShield);
            Assert.True(pendingQuen.IsPending);
            Assert.Contains(pendingQuen, iceTroll.Effects);
        }

        [Fact]
        public async Task ReturningToHandDuringDeployDoesNotConsumePendingQuen()
        {
            var fixture = new HeadlessGameFixture();
            var unit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.MyHand);
            HeadlessGameFixture.ReplaceMainEffect(unit, new ReturnSelfDuringDeploy(unit));
            var pendingQuen = new PendingQuenEffect(unit);
            unit.Effects.Add(pendingQuen);
            await fixture.SynchronizeClientsAsync();

            await unit.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.Equal(RowPosition.MyHand, unit.Status.CardRow);
            Assert.False(unit.Status.IsShield);
            Assert.Equal(0, unit.Status.HealthStatus);
            Assert.True(pendingQuen.IsPending);
            Assert.Contains(pendingQuen, unit.Effects);
        }

        [Fact]
        public async Task EnemySideLandingKeepsQuenPendingUntilOneFriendlyLanding()
        {
            var fixture = new HeadlessGameFixture();
            var unit = fixture.AddCard(
                fixture.Game.Player1Index,
                CardId.IceTroll,
                RowPosition.EnemyRow1);
            var pendingQuen = new PendingQuenEffect(unit);
            unit.Effects.Add(pendingQuen);
            await fixture.SynchronizeClientsAsync();

            await unit.Effect.CardDown(
                false,
                false,
                false,
                (false, false));

            Assert.True(pendingQuen.IsPending);
            Assert.False(unit.Status.IsShield);
            Assert.Equal(0, unit.Status.HealthStatus);

            await fixture.Game.ShowCardMove(
                new CardLocation(RowPosition.MyRow1, 0),
                unit);
            await unit.Effect.CardDown(
                false,
                false,
                false,
                (false, false));

            Assert.False(pendingQuen.IsPending);
            Assert.True(unit.Status.IsShield);
            Assert.Equal(2, unit.Status.HealthStatus);

            await unit.Effect.CardDown(
                false,
                false,
                false,
                (false, false));
            Assert.Equal(2, unit.Status.HealthStatus);
        }

        private sealed class ReturnSelfDuringDeploy : CardEffect
        {
            public ReturnSelfDuringDeploy(GameCard card) : base(card)
            {
            }

            public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
            {
                await Game.ShowCardMove(
                    new CardLocation(
                        RowPosition.MyHand,
                        Game.PlayersHandCard[PlayerIndex].Count),
                    Card);
                return 0;
            }
        }
    }
}
