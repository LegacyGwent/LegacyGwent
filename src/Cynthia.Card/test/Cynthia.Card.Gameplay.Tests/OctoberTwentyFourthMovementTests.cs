using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class OctoberTwentyFourthMovementTests
    {
        [Fact]
        public async Task BowDryadRetainsMovementTriggeredByAnEarlierTurnStartListener()
        {
            var f = new HeadlessGameFixture();
            var earlierListener = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            var dryad = f.AddCard(f.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            HeadlessGameFixture.ReplaceMainEffect(earlierListener,
                new TurnStartMover(earlierListener, enemy));
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();

            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            Assert.Equal(RowPosition.MyRow2, enemy.Status.CardRow);
            Assert.Equal(0, enemy.Status.HealthStatus);
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(-3, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task GezrasUsesEightUnusedMovesAndOnlyTargetsEnemies()
        {
            var f = new HeadlessGameFixture();
            var ally = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var gezras = f.AddCard(f.Game.Player1Index, CardId.GezrasOfLeyda, RowPosition.MyRow2);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            await f.SynchronizeClientsAsync();

            await gezras.Effect.CardPlayEffect(false, false);

            Assert.Equal(20, ally.Status.Strength);
            Assert.Equal(12, enemy.Status.Strength);
        }

        [Fact]
        public async Task GezrasCanMoveEightAlliesWithoutWeakening()
        {
            var f = new HeadlessGameFixture();
            var allies = Enumerable.Range(0, 8)
                .Select(index => f.AddCard(f.Game.Player1Index, CardId.ElvenScout,
                    index < 4 ? RowPosition.MyRow1 : RowPosition.MyRow3)).ToArray();
            var gezras = f.AddCard(f.Game.Player1Index, CardId.GezrasOfLeyda, RowPosition.MyRow2);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 20);
            await f.SynchronizeClientsAsync();

            await gezras.Effect.CardPlayEffect(false, false);

            Assert.All(allies, ally => Assert.Equal(RowPosition.MyRow2, ally.Status.CardRow));
            Assert.Equal(20, enemy.Status.Strength);
            Assert.Equal(8, f.FirstPlayer.MenuRequests.Single().SelectCount);
        }

        [Fact]
        public async Task GezrasLeavesAlliesUntouchedWhenNoEnemyCanBeTargeted()
        {
            var f = new HeadlessGameFixture();
            var ally = f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 20);
            var gezras = f.AddCard(f.Game.Player1Index, CardId.GezrasOfLeyda, RowPosition.MyRow2);
            await f.SynchronizeClientsAsync();
            await gezras.Effect.CardPlayEffect(false, false);
            Assert.Equal(20, ally.Status.Strength);
            Assert.Empty(f.FirstPlayer.PlaceSelectionSources);
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async Task BowDryadWaitsUntilTurnEndAfterSelfOrEnemyMovesToRanged(bool moveSelf)
        {
            var f = new HeadlessGameFixture();
            var dryad = f.AddCard(f.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();
            await dryad.Effects.RaiseEvent(new CardPlayEffect(false, false));
            Assert.Equal(-3, enemy.Status.HealthStatus);

            var mover = moveSelf ? dryad : enemy;
            await f.Game.AddTask(async () =>
                await mover.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad));
            Assert.Equal(-3, enemy.Status.HealthStatus);
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(-6, enemy.Status.HealthStatus);
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(-6, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task BowDryadRepeatsOnceAfterSeveralQualifyingMovesEvenIfMoverLeavesRanged()
        {
            var f = new HeadlessGameFixture();
            var dryad = f.AddCard(f.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();

            await f.Game.AddTask(async () =>
            {
                await enemy.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad);
                await dryad.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad);
                await enemy.Effect.Move(new CardLocation(RowPosition.MyRow3, 0), dryad);
                await dryad.Effect.Move(new CardLocation(RowPosition.MyRow3, 0), dryad);
            });
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(-3, enemy.Status.HealthStatus);
        }

        [Theory]
        [InlineData(true, RowPosition.MyRow2)]
        [InlineData(false, RowPosition.MyRow3)]
        public async Task BowDryadIgnoresOtherAlliedMovesAndEnemyMovesOutsideRanged(
            bool alliedMover, RowPosition destination)
        {
            var f = new HeadlessGameFixture();
            var dryad = f.AddCard(f.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            var mover = alliedMover
                ? f.AddCard(f.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1, 30) : enemy;
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () =>
                await mover.Effect.Move(new CardLocation(destination, 0), dryad));
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, enemy.Status.HealthStatus);
        }

        [Fact]
        public async Task BowDryadIgnoresEnemyPlayedDirectlyToRangedAndOpponentTurnMovement()
        {
            var f = new HeadlessGameFixture();
            var dryad = f.AddCard(f.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyHand, 30);
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();
            await enemy.Effect.Play(new CardLocation(RowPosition.MyRow2, 0));
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, enemy.Status.HealthStatus);

            f.Game.GameRound = (TwoPlayer)f.Game.Player2Index;
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player2Index));
            await f.Game.AddTask(async () =>
                await dryad.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad));
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player2Index));
            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, enemy.Status.HealthStatus);
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task BowDryadClearsPendingRepeatOnUnlockWithoutPreventingNewMovement(bool crossTurnBoundary)
        {
            var f = new HeadlessGameFixture();
            var dryad = f.AddCard(f.Game.Player1Index, CardId.BowDryad, RowPosition.MyRow1);
            var enemy = f.AddCard(f.Game.Player2Index, CardId.Wolf, RowPosition.MyRow1, 30);
            f.Game.GameRound = (TwoPlayer)f.Game.Player1Index;
            await f.SynchronizeClientsAsync();
            await f.Game.AddTask(async () =>
                await dryad.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad));
            await dryad.Effect.Lock(enemy);
            if (crossTurnBoundary)
            {
                await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
                await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            }
            await dryad.Effect.Lock(enemy);
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(0, enemy.Status.HealthStatus);

            await f.Game.SendEvent(new AfterTurnStart(f.Game.Player1Index));
            await f.Game.AddTask(async () =>
                await enemy.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), dryad));
            await f.Game.SendEvent(new AfterTurnOver(f.Game.Player1Index));
            Assert.Equal(-3, enemy.Status.HealthStatus);
        }

        private sealed class TurnStartMover : CardEffect, IHandlesEvent<AfterTurnStart>
        {
            private readonly GameCard _target;

            public TurnStartMover(GameCard card, GameCard target) : base(card) => _target = target;

            public async Task HandleEvent(AfterTurnStart @event)
            {
                if (@event.PlayerIndex == PlayerIndex)
                    await _target.Effect.Move(new CardLocation(RowPosition.MyRow2, 0), Card);
            }
        }
    }
}
