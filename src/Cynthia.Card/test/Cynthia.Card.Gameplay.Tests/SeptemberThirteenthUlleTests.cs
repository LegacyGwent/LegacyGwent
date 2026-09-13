using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberThirteenthUlleTests
    {
        private const string UlleId = "70178";

        [Fact]
        public async Task UlleResurrectsOnOwnerTurnStartAndCountsResurrectionsInsteadOfTurns()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyCemetery);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player2Index));
            Assert.True(ulle.Status.CardRow.IsInCemetery());
            Assert.Equal(2, ulle.Status.Countdown);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(ulle.Status.CardRow.IsOnPlace());
            Assert.Equal(1, ulle.Status.Countdown);
            Assert.Equal(3, ulle.Status.Strength);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            Assert.Equal(1, ulle.Status.Countdown);
            Assert.Equal(3, ulle.Status.Strength);
            Assert.False(ulle.Status.IsLock);
            Assert.True(enemy.Status.CardRow.IsOnPlace());
            Assert.Equal(0, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task UlleWeakensEverySecondResurrectionAcrossRepeatedDuelDeathsAndBanishesAtZero()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyCemetery);
            fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            await fixture.SynchronizeClientsAsync();

            for (var resurrection = 1; resurrection <= 6; resurrection++)
            {
                await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));

                var expectedStrength = 3 - resurrection / 2;
                Assert.Equal(expectedStrength, ulle.Status.Strength);
                Assert.Equal(0, ulle.Status.HealthStatus);
                Assert.Equal(resurrection % 2 == 1 ? 1 : 2, ulle.Status.Countdown);
                Assert.True(ulle.Status.IsCountdown);
                if (expectedStrength == 0)
                {
                    Assert.Equal(RowPosition.Banish, ulle.Status.CardRow);
                    break;
                }

                Assert.True(ulle.Status.CardRow.IsOnPlace());
                await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
                Assert.True(ulle.Status.CardRow.IsInCemetery());
                Assert.Equal(expectedStrength, ulle.Status.Strength);
                Assert.False(ulle.Status.IsLock);
            }

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.Equal(RowPosition.Banish, ulle.Status.CardRow);
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task UlleCountsExternalResurrectionButNotOtherCardsOrFailedAttempts()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyCemetery);
            var other = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(() => other.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), other));
            Assert.Equal(2, ulle.Status.Countdown);

            await fixture.Game.AddTask(() => ulle.Effect.Resurrect(
                new CardLocation(RowPosition.MyStay, 0), other));
            Assert.Equal(1, ulle.Status.Countdown);
            await fixture.Game.AddTask(() => ulle.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), other));
            Assert.True(ulle.Status.CardRow.IsOnStay());
            Assert.Equal(1, ulle.Status.Countdown);

            await ulle.Effect.ToCemetery();
            await fixture.Game.AddTask(() => ulle.Effect.Resurrect(
                new CardLocation(RowPosition.MyHand, 0), other));
            Assert.True(ulle.Status.CardRow.IsInHand());
            Assert.Equal(2, ulle.Status.Strength);
            Assert.Equal(0, ulle.Status.HealthStatus);
            Assert.Equal(2, ulle.Status.Countdown);
        }

        [Fact]
        public async Task UlleCannotSpendAResurrectionCountWhenAllRowsAreFull()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.RowMaxCount = 1;
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyCemetery);
            var blocker = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow3);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(ulle.Status.CardRow.IsInCemetery());
            Assert.Equal(2, ulle.Status.Countdown);
            Assert.Equal(3, ulle.Status.Strength);

            await blocker.Effect.ToCemetery();
            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(ulle.Status.CardRow.IsOnPlace());
            Assert.Equal(1, ulle.Status.Countdown);
            Assert.Equal(3, ulle.Status.Strength);
        }

        [Fact]
        public async Task UlleDuelsOneTiedWeakestEnemyAndThenLockSuppressesFurtherTurns()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyRow1);
            var weakest = new[]
            {
                fixture.AddCard(fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 2),
                fixture.AddCard(fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow2, 2)
            };
            var stronger = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow3, 7);
            var ally = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2, 1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Single(weakest, card => card.Status.CardRow.IsInCemetery());
            Assert.Single(weakest, card => card.Status.CardRow.IsOnPlace());
            Assert.True(stronger.Status.CardRow.IsOnPlace());
            Assert.Equal(0, stronger.Status.HealthStatus);
            Assert.True(ally.Status.CardRow.IsOnPlace());
            Assert.True(ulle.Status.CardRow.IsOnPlace());
            Assert.True(ulle.Status.IsLock);
            Assert.Equal(2, ulle.Status.Countdown);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Single(weakest, card => card.Status.CardRow.IsInCemetery());
            Assert.True(ulle.Status.IsLock);
        }

        [Fact]
        public async Task LockedUlleStaysLockedInTheGraveyardUntilExternalResurrectionUnlocksIt()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyCemetery);
            var source = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyHand);
            var observer = new UnlockObserver(source, ulle);
            HeadlessGameFixture.ReplaceMainEffect(source, observer);
            fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.True(ulle.Status.IsLock);
            Assert.Equal(1, ulle.Status.Countdown);
            await ulle.Effect.ToCemetery();
            Assert.True(ulle.Status.IsLock);

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            Assert.True(ulle.Status.CardRow.IsInCemetery());
            Assert.Equal(1, ulle.Status.Countdown);

            await fixture.Game.AddTask(() => ulle.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), source));

            Assert.True(observer.UnlockedInCemeteryBeforeCounting);
            Assert.False(ulle.Status.IsLock);
            Assert.True(ulle.Status.CardRow.IsOnPlace());
            Assert.Equal(2, ulle.Status.Strength);
            Assert.Equal(2, ulle.Status.Countdown);
        }

        [Fact]
        public async Task SurvivingUlleTogglesBackToUnlockedIfAnEffectLockedItDuringTheDuel()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyRow1);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1, 1);
            var lockAttacker = new LockAttackerBeforeDamage(enemy);
            HeadlessGameFixture.ReplaceMainEffect(enemy, lockAttacker);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.True(lockAttacker.LockedAttacker);
            Assert.True(enemy.Status.CardRow.IsInCemetery());
            Assert.True(ulle.Status.CardRow.IsOnPlace());
            Assert.False(ulle.Status.IsLock);
        }

        [Fact]
        public async Task UlleWithoutAnEnemyDoesNotToggleItsLockOrSpendAResurrectionCount()
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, RowPosition.MyRow1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.True(ulle.Status.CardRow.IsOnPlace());
            Assert.False(ulle.Status.IsLock);
            Assert.Equal(2, ulle.Status.Countdown);
        }

        [Theory]
        [InlineData(RowPosition.MyHand)]
        [InlineData(RowPosition.MyDeck)]
        public async Task UlleDoesNotResurrectOrDuelFromHandOrDeck(RowPosition row)
        {
            var fixture = new HeadlessGameFixture();
            var ulle = fixture.AddCard(fixture.Game.Player1Index, UlleId, row);
            var enemy = fixture.AddCard(fixture.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 1);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnStart(fixture.Game.Player1Index));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(row, ulle.Status.CardRow);
            Assert.False(ulle.Status.IsLock);
            Assert.Equal(2, ulle.Status.Countdown);
            Assert.Equal(0, enemy.Status.HealthStatus);
            Assert.True(enemy.Status.CardRow.IsOnPlace());
        }

        private sealed class UnlockObserver : CardEffect, IHandlesEvent<AfterCardUnLock>
        {
            private readonly GameCard _ulle;

            public UnlockObserver(GameCard card, GameCard ulle) : base(card) => _ulle = ulle;

            public bool UnlockedInCemeteryBeforeCounting { get; private set; }

            public Task HandleEvent(AfterCardUnLock @event)
            {
                if (@event.Target == _ulle)
                {
                    UnlockedInCemeteryBeforeCounting = _ulle.Status.CardRow.IsInCemetery()
                        && _ulle.Status.Countdown == 1;
                }
                return Task.CompletedTask;
            }
        }

        private sealed class LockAttackerBeforeDamage : CardEffect
        {
            public LockAttackerBeforeDamage(GameCard card) : base(card) { }

            public bool LockedAttacker { get; private set; }

            public override async Task Damage(int num, GameCard source,
                BulletType showType = BulletType.Arrow, bool isPenetrate = false,
                DamageType damageType = DamageType.Unit)
            {
                await source.Effect.Lock(Card);
                LockedAttacker = source.Status.IsLock;
                await base.Damage(num, source, showType, isPenetrate, damageType);
            }
        }
    }
}
