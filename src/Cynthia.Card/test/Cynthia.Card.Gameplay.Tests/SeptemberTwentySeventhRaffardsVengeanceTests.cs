using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberTwentySeventhRaffardsVengeanceTests
    {
        private const string Raffard = "70212";

        [Fact]
        public async Task OwnerTurnEndsCountFromFourAndFireOnceWithoutCrew()
        {
            var fixture = new HeadlessGameFixture();
            var raffard = fixture.AddCard(fixture.Game.Player1Index, Raffard, RowPosition.MyHand);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 50);
            fixture.FirstPlayer.PlaceSelectionOverride = info => info.CanSelect.CardsPartToLocation()
                .Where(location => location.RowPosition.IsOnPlace() && !location.RowPosition.IsMyRow())
                .Take(info.SelectCount).ToList();
            await fixture.SynchronizeClientsAsync();

            await raffard.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));
            Assert.True(raffard.Status.IsCountdown);
            Assert.Equal(4, raffard.Status.Countdown);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            Assert.Equal(4, raffard.Status.Countdown);
            for (var expected = 3; expected >= 1; expected--)
            {
                await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
                Assert.Equal(expected, raffard.Status.Countdown);
                Assert.Equal(0, enemy.Status.HealthStatus);
            }

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(4, raffard.Status.Countdown);
            Assert.Equal(-10, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task TwoCrewReduceDeployCountdownButOrdinaryTurnDoesNotReapplyThem()
        {
            var fixture = new HeadlessGameFixture();
            var left = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            left.Status.CrewCount = 1;
            var raffard = fixture.AddCard(fixture.Game.Player1Index, Raffard, RowPosition.MyHand);
            var right = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            right.Status.CrewCount = 1;
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 50);
            fixture.FirstPlayer.PlaceSelectionOverride = info => info.CanSelect.CardsPartToLocation()
                .Where(location => location.RowPosition.IsOnPlace() && !location.RowPosition.IsMyRow())
                .Take(info.SelectCount).ToList();
            await fixture.SynchronizeClientsAsync();

            await raffard.Effect.Play(new CardLocation(RowPosition.MyRow1, 1));
            Assert.Equal(2, raffard.Status.Countdown);
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(1, raffard.Status.Countdown);
            Assert.Equal(0, enemy.Status.HealthStatus);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(-10, enemy.Status.HealthStatus);
            Assert.Equal(2, raffard.Status.Countdown);
        }

        [Fact]
        public async Task SiegeMasterAtOneFiresThenAppliesRemainingCrewStepWithoutRecursion()
        {
            var fixture = new HeadlessGameFixture();
            var left = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            left.Status.CrewCount = 1;
            var raffard = fixture.AddCard(fixture.Game.Player1Index, Raffard, RowPosition.MyHand);
            var right = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            right.Status.CrewCount = 1;
            var mageId = GwentMap.CardMap.First(entry => entry.Value.CardType == CardType.Unit &&
                entry.Value.Categories.Contains(Categorie.Mage)).Key;
            var alliedMage = fixture.AddCard(fixture.Game.Player1Index, mageId, RowPosition.MyRow1);
            var damageObserver = new CountdownDuringDamage(alliedMage, raffard);
            HeadlessGameFixture.ReplaceMainEffect(alliedMage, damageObserver);
            var spyingMage = fixture.AddCard(fixture.Game.Player1Index, mageId, RowPosition.MyRow1);
            spyingMage.PlayerIndex = fixture.Game.Player2Index;
            spyingMage.Status.IsSpying = true;
            fixture.AddCard(fixture.Game.Player1Index, mageId, RowPosition.MyRow2);
            var enemyMage = fixture.AddCard(fixture.Game.Player2Index, mageId, RowPosition.MyRow1, 50);
            var otherEnemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 50);
            var siegeMaster = fixture.AddCard(fixture.Game.Player1Index, CardId.SiegeMaster, RowPosition.MyRow2);
            fixture.FirstPlayer.PlaceSelectionOverride = info =>
            {
                var candidates = info.CanSelect.CardsPartToLocation();
                var enemy = candidates.Where(location => location.RowPosition.IsOnPlace() &&
                    !location.RowPosition.IsMyRow()).Take(info.SelectCount).ToList();
                return enemy.Count > 0 ? enemy : candidates.Take(info.SelectCount).ToList();
            };
            await fixture.SynchronizeClientsAsync();

            await raffard.Effect.Play(new CardLocation(RowPosition.MyRow1, 1));
            Assert.Equal(2, raffard.Status.Countdown);
            await raffard.Effect.SetCountdown(1);
            // The target is the only allied Machine, so this exercises Siege Master's
            // real Deploy replay rather than calling Raffard's helper directly.
            await siegeMaster.Effect.CardPlayEffect(false, false);

            Assert.Equal(3, raffard.Status.Countdown);
            Assert.Equal(-12, enemyMage.Status.HealthStatus);
            Assert.Equal(0, otherEnemy.Status.HealthStatus);
            Assert.Equal(new[] { 0 }, damageObserver.SeenCountdowns);
        }

        private sealed class CountdownDuringDamage : CardEffect, IHandlesEvent<BeforeCardDamage>
        {
            private readonly GameCard source;
            public System.Collections.Generic.List<int> SeenCountdowns { get; } = new System.Collections.Generic.List<int>();

            public CountdownDuringDamage(GameCard host, GameCard source) : base(host) => this.source = source;

            public Task HandleEvent(BeforeCardDamage damage)
            {
                if (damage.Source == source) SeenCountdowns.Add(source.Status.Countdown);
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task LockedCrewIsIgnoredLockedRaffardPausesAndNoTargetStillResets()
        {
            var fixture = new HeadlessGameFixture();
            var left = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            left.Status.CrewCount = 1;
            var raffard = fixture.AddCard(fixture.Game.Player1Index, Raffard, RowPosition.MyHand);
            var right = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            right.Status.CrewCount = 1;
            right.Status.IsLock = true;
            await fixture.SynchronizeClientsAsync();

            await raffard.Effect.Play(new CardLocation(RowPosition.MyRow1, 1));
            Assert.Equal(3, raffard.Status.Countdown);
            raffard.Status.IsLock = true;
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(3, raffard.Status.Countdown);
            await raffard.Effect.CardPlayEffect(false, false);
            Assert.Equal(3, raffard.Status.Countdown);

            raffard.Status.IsLock = false;
            await raffard.Effect.SetCountdown(1);
            // Crew still counts, while immunity makes the other units untargetable.
            left.Status.IsImmue = right.Status.IsImmue = true;
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(3, raffard.Status.Countdown);
            Assert.True(raffard.Status.IsCountdown);
        }

        [Fact]
        public async Task UnqualifiedDamageCanTargetAnAlliedUnit()
        {
            var fixture = new HeadlessGameFixture();
            var ally = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 50);
            var raffard = fixture.AddCard(fixture.Game.Player1Index, Raffard, RowPosition.MyHand);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 50);
            fixture.FirstPlayer.PlaceSelectionOverride = info => info.CanSelect.CardsPartToLocation()
                .Where(location => location.RowPosition.IsMyRow())
                .Take(info.SelectCount).ToList();
            await fixture.SynchronizeClientsAsync();

            await raffard.Effect.Play(new CardLocation(RowPosition.MyRow1, 1));
            await raffard.Effect.SetCountdown(1);
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(-10, ally.Status.HealthStatus);
            Assert.Equal(0, enemy.Status.HealthStatus);
            Assert.Equal(4, raffard.Status.Countdown);
        }
    }
}
