using System;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class GameplayReliabilityTests
    {
        [Fact]
        public async Task ThreeSyannasResolveThroughTheProductionEventPipeline()
        {
            var fixture = new HeadlessGameFixture();
            var target = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var enemy = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            var first = fixture.AddCard(fixture.Game.Player1Index, "70025", RowPosition.MyRow2);
            var second = fixture.AddCard(fixture.Game.Player1Index, "70025", RowPosition.MyRow2);
            var third = fixture.AddCard(fixture.Game.Player1Index, "70025", RowPosition.MyRow2);
            await fixture.SynchronizeClientsAsync();

            foreach (var syanna in new[] { first, second, third })
            {
                await syanna.Effect.CardPlayEffect(false, false);
                await syanna.Effect.Damage(1, enemy);
            }

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index))
                .WaitAsync(TimeSpan.FromSeconds(2));
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index))
                .WaitAsync(TimeSpan.FromSeconds(2));

            Assert.Equal(-3, target.Status.HealthStatus);
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task RecursiveCardEventsFailTheMatchChainWithoutPoisoningItsPipeline()
        {
            var fixture = new HeadlessGameFixture();
            var card = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyRow1);
            HeadlessGameFixture.ReplaceMainEffect(card, new RecursiveTurnEndEffect(card));
            await fixture.SynchronizeClientsAsync();

            await Assert.ThrowsAsync<GameResolutionLimitException>(() =>
                fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index)));

            Assert.False(fixture.Game.OperactionList.IsRunning);
            card.Status.IsLock = true;
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task PipelineDropsAnOverflowedChainAndCanRunNewWork()
        {
            var pipeline = new Pipeline(maxTasksPerDrain: 3);
            Func<Task> repeat = null;
            repeat = () => pipeline.AddLast(repeat);

            await Assert.ThrowsAsync<GameResolutionLimitException>(() => pipeline.AddLast(repeat));
            Assert.False(pipeline.IsRunning);

            var executed = false;
            await pipeline.AddLast(() =>
            {
                executed = true;
                return Task.CompletedTask;
            });

            Assert.True(executed);
            Assert.False(pipeline.IsRunning);
        }

        [Fact]
        public async Task ResolutionAbortTerminatesOnlyTheCurrentGame()
        {
            var failedFixture = new HeadlessGameFixture();
            await failedFixture.Game.AbortDueToResolutionError(new InvalidOperationException("synthetic"));
            Assert.True(failedFixture.Game.IsTerminated);

            var healthyFixture = new HeadlessGameFixture();
            await healthyFixture.Game.SendEvent(new AfterTurnOver(healthyFixture.Game.Player1Index));
            Assert.False(healthyFixture.Game.IsTerminated);
            Assert.False(healthyFixture.Game.OperactionList.IsRunning);
        }

        private sealed class RecursiveTurnEndEffect : CardEffect, IHandlesEvent<AfterTurnOver>
        {
            public RecursiveTurnEndEffect(GameCard card) : base(card) { }

            public override Task<int> CardPlayEffect(bool isSpying, bool isReveal)
                => Task.FromResult(0);

            public async Task HandleEvent(AfterTurnOver @event)
            {
                await Game.SendEvent(new AfterTurnOver(@event.PlayerIndex));
            }
        }
    }
}
