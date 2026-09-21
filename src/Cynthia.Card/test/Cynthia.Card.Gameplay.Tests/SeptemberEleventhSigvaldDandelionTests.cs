using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SeptemberEleventhSigvaldDandelionTests
    {
        [Fact]
        public async Task SigvaldCountsSuccessfulResurrectionsInsteadOfTurnEnds()
        {
            var fixture = new HeadlessGameFixture();
            var sigvald = fixture.AddCard(fixture.Game.Player1Index, "70038", RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player2Index));
            Assert.True(sigvald.Status.CardRow.IsInCemetery());
            Assert.Equal(2, sigvald.Status.Countdown);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.True(sigvald.Status.CardRow.IsOnPlace());
            Assert.Equal(1, sigvald.Status.Strength);
            Assert.Equal(1, sigvald.Status.Countdown);

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(1, sigvald.Status.Strength);
            Assert.Equal(1, sigvald.Status.Countdown);

            await sigvald.Effect.ToCemetery();
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.True(sigvald.Status.CardRow.IsOnPlace());
            Assert.Equal(2, sigvald.Status.Strength);
            Assert.Equal(2, sigvald.Status.Countdown);
            Assert.True(sigvald.Status.IsCountdown);
        }

        [Theory]
        [InlineData(6, 7)]
        [InlineData(7, 8)]
        [InlineData(8, 9)]
        public async Task SigvaldStrengthensEverySecondResurrectionWithoutPowerCap(
            int strength, int expectedStrength)
        {
            var fixture = new HeadlessGameFixture();
            var sigvald = fixture.AddCard(
                fixture.Game.Player1Index, "70038", RowPosition.MyCemetery, strength);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.Equal(strength, sigvald.Status.Strength);
            await sigvald.Effect.ToCemetery();
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.True(sigvald.Status.CardRow.IsOnPlace());
            Assert.Equal(expectedStrength, sigvald.Status.Strength);
            Assert.Equal(2, sigvald.Status.Countdown);
        }

        [Fact]
        public async Task SigvaldCountsExternalResurrectionsButNotOtherUnitsOrFailedAttempts()
        {
            var fixture = new HeadlessGameFixture();
            var sigvald = fixture.AddCard(fixture.Game.Player1Index, "70038", RowPosition.MyCemetery);
            var other = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.AddTask(() => other.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), other));
            Assert.Equal(2, sigvald.Status.Countdown);

            await fixture.Game.AddTask(() => sigvald.Effect.Resurrect(
                new CardLocation(RowPosition.MyStay, 0), other));
            Assert.Equal(1, sigvald.Status.Countdown);
            await fixture.Game.AddTask(() => sigvald.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), other));
            Assert.Equal(1, sigvald.Status.Countdown);

            await sigvald.Effect.ToCemetery();
            await fixture.Game.AddTask(() => sigvald.Effect.Resurrect(
                new CardLocation(RowPosition.MyRow1, 0), other));
            Assert.Equal(2, sigvald.Status.Strength);
            Assert.Equal(2, sigvald.Status.Countdown);
        }

        [Fact]
        public async Task SigvaldFullBoardFailureDoesNotConsumeItsResurrectionCounter()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.RowMaxCount = 1;
            var sigvald = fixture.AddCard(fixture.Game.Player1Index, "70038", RowPosition.MyCemetery);
            var blocker = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow1);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow3);
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.True(sigvald.Status.CardRow.IsInCemetery());
            Assert.Equal(2, sigvald.Status.Countdown);

            await blocker.Effect.ToCemetery();
            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));
            Assert.True(sigvald.Status.CardRow.IsOnPlace());
            Assert.Equal(1, sigvald.Status.Countdown);
            Assert.Equal(1, sigvald.Status.Strength);
        }

        [Theory]
        [InlineData(RowPosition.MyHand, false)]
        [InlineData(RowPosition.MyDeck, false)]
        [InlineData(RowPosition.MyCemetery, true)]
        public async Task SigvaldDoesNotSelfResurrectOutsideTheGraveyardOrWhileLocked(
            RowPosition row, bool locked)
        {
            var fixture = new HeadlessGameFixture();
            var sigvald = fixture.AddCard(fixture.Game.Player1Index, "70038", row);
            sigvald.Status.IsLock = locked;
            await fixture.SynchronizeClientsAsync();

            await fixture.Game.SendEvent(new AfterTurnOver(fixture.Game.Player1Index));

            Assert.Equal(row, sigvald.Status.CardRow);
            Assert.Equal(2, sigvald.Status.Countdown);
            Assert.Equal(1, sigvald.Status.Strength);
        }

        [Theory]
        [InlineData(CardId.GeraltOfRivia, 0)]
        [InlineData(CardId.TrissMerigold, -9)]
        public async Task DandelionPlaysTheSelectedExactHandCardAndResolvesItsDeployBeforeDrawing(
            string selectedId, int dandelionHealthAtDraw)
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var dandelion = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DandelionVainglory, RowPosition.MyHand);
            var geraltVariant = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltProfessional, RowPosition.MyHand);
            var trissVariant = fixture.AddCard(
                fixture.Game.Player1Index, CardId.TrissTelekinesis, RowPosition.MyHand);
            var geralt = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            var triss = fixture.AddCard(
                fixture.Game.Player1Index, CardId.TrissMerigold, RowPosition.MyHand);
            var selected = selectedId == CardId.GeraltOfRivia ? geralt : triss;
            var unselected = selected == geralt ? triss : geralt;
            var draw = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            var observer = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyCemetery);
            var recorder = new PlayAndDrawRecorder(observer, selected, dandelion, draw);
            HeadlessGameFixture.ReplaceMainEffect(observer, recorder);
            fixture.FirstPlayer.QueueMenuCardIds(selectedId);
            await fixture.SynchronizeClientsAsync();

            await dandelion.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(selected.Status.CardRow.IsOnPlace());
            Assert.True(unselected.Status.CardRow.IsInHand());
            Assert.True(geraltVariant.Status.CardRow.IsInHand());
            Assert.True(trissVariant.Status.CardRow.IsInHand());
            Assert.True(draw.Status.CardRow.IsInHand());
            var menu = Assert.Single(fixture.FirstPlayer.MenuRequests);
            Assert.Equal(new[] { CardId.GeraltOfRivia, CardId.TrissMerigold },
                menu.SelectList.Select(card => card.CardId));
            Assert.Equal(new[] { "play-from-hand", "draw" }, recorder.Events);
            Assert.Equal(dandelionHealthAtDraw, recorder.DandelionHealthAtDraw);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task DandelionWithoutEitherExactCardInHandDoesNotDrawOrPlayCharacterVariants()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var dandelion = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DandelionVainglory, RowPosition.MyHand);
            var variants = new[]
            {
                fixture.AddCard(fixture.Game.Player1Index, CardId.GeraltProfessional, RowPosition.MyHand),
                fixture.AddCard(fixture.Game.Player1Index, CardId.TrissTelekinesis, RowPosition.MyHand)
            };
            var deckGeralt = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var cemeteryTriss = fixture.AddCard(
                fixture.Game.Player1Index, CardId.TrissMerigold, RowPosition.MyCemetery);
            var enemyGeralt = fixture.AddCard(
                fixture.Game.Player2Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await dandelion.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.All(variants, card => Assert.True(card.Status.CardRow.IsInHand()));
            Assert.True(deckGeralt.Status.CardRow.IsInDeck());
            Assert.True(cemeteryTriss.Status.CardRow.IsInCemetery());
            Assert.True(enemyGeralt.Status.CardRow.IsInHand());
            Assert.Empty(fixture.FirstPlayer.MenuRequests);
            Assert.Equal(0, dandelion.Status.HealthStatus);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
        }

        [Fact]
        public async Task DandelionDoesNotRequestAnImpossibleHandPlayOnAFullBoard()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.RowMaxCount = 1;
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow2);
            fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyRow3);
            var dandelion = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DandelionVainglory, RowPosition.MyHand);
            var geralt = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            var draw = fixture.AddCard(fixture.Game.Player1Index, CardId.Wolf, RowPosition.MyDeck);
            await fixture.SynchronizeClientsAsync();

            await dandelion.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(geralt.Status.CardRow.IsInHand());
            Assert.True(draw.Status.CardRow.IsInDeck());
            Assert.Empty(fixture.FirstPlayer.MenuRequests);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        [Fact]
        public async Task DandelionStillPlaysTheHandCardWhenTheDeckIsEmpty()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[fixture.Game.Player1Index].Clear();
            var dandelion = fixture.AddCard(
                fixture.Game.Player1Index, CardId.DandelionVainglory, RowPosition.MyHand);
            var geralt = fixture.AddCard(
                fixture.Game.Player1Index, CardId.GeraltOfRivia, RowPosition.MyHand);
            await fixture.SynchronizeClientsAsync();

            await dandelion.Effect.Play(new CardLocation(RowPosition.MyRow1, 0));

            Assert.True(geralt.Status.CardRow.IsOnPlace());
            Assert.Empty(fixture.Game.PlayersHandCard[fixture.Game.Player1Index]);
            Assert.Empty(fixture.Game.PlayersStay[fixture.Game.Player1Index]);
            Assert.False(fixture.Game.OperactionList.IsRunning);
        }

        private sealed class PlayAndDrawRecorder : CardEffect,
            IHandlesEvent<AfterUnitPlay>, IHandlesEvent<AfterPlayerDraw>
        {
            private readonly GameCard _selected;
            private readonly GameCard _dandelion;
            private readonly GameCard _draw;

            public PlayAndDrawRecorder(GameCard card, GameCard selected, GameCard dandelion, GameCard draw)
                : base(card)
            {
                _selected = selected;
                _dandelion = dandelion;
                _draw = draw;
            }

            public IList<string> Events { get; } = new List<string>();
            public int? DandelionHealthAtDraw { get; private set; }

            public Task HandleEvent(AfterUnitPlay @event)
            {
                if (@event.PlayedCard == _selected)
                {
                    Events.Add(@event.IsFromHand ? "play-from-hand" : "play-from-other-zone");
                }
                return Task.CompletedTask;
            }

            public Task HandleEvent(AfterPlayerDraw @event)
            {
                if (@event.DrawCard == _draw)
                {
                    Events.Add("draw");
                    DandelionHealthAtDraw = _dandelion.Status.HealthStatus;
                }
                return Task.CompletedTask;
            }
        }
    }
}
