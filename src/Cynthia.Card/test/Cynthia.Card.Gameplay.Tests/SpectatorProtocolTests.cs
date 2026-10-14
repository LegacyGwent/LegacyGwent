using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Cynthia.Card.Server;
using Newtonsoft.Json;
using Xunit;

namespace Cynthia.Card.Gameplay.Tests
{
    public class SpectatorProtocolTests
    {
        [Fact]
        public void ExistingWireIdsRemainStable()
        {
            var existing = new[] {
                ServerOperationType.GameStart,
                ServerOperationType.GetDragOrPass,
                ServerOperationType.RoundEnd,
                ServerOperationType.GameEnd,
                ServerOperationType.EndOfBigRound,
                ServerOperationType.CardsToCemetery,
                ServerOperationType.Debug,
                ServerOperationType.MessageBox,
                ServerOperationType.ClientDelay,
                ServerOperationType.RemindYouRoundStart,
                ServerOperationType.BigRoundShowPoint,
                ServerOperationType.BigRoundSetMessage,
                ServerOperationType.BigRoundShowClose,
                ServerOperationType.MulliganStart,
                ServerOperationType.MulliganData,
                ServerOperationType.GetMulliganInfo,
                ServerOperationType.MulliganEnd,
                ServerOperationType.SelectMenuCards,
                ServerOperationType.SelectPlaceCards,
                ServerOperationType.SelectRow,
                ServerOperationType.PlayCard,
                ServerOperationType.CardMove,
                ServerOperationType.CardOn,
                ServerOperationType.CardDown,
                ServerOperationType.CreateCard,
                ServerOperationType.ShowWeatherApply,
                ServerOperationType.ShowCardIconEffect,
                ServerOperationType.ShowCardBreakEffect,
                ServerOperationType.ShowCardNumberChange,
                ServerOperationType.ShowBullet,
                ServerOperationType.SetCard,
                ServerOperationType.SetMulliganInfo,
                ServerOperationType.SetCoinInfo,
                ServerOperationType.SetAllInfo,
                ServerOperationType.SetGameInfo,
                ServerOperationType.SetCardsInfo,
                ServerOperationType.SetPointInfo,
                ServerOperationType.SetCountInfo,
                ServerOperationType.SetPassInfo,
                ServerOperationType.SetWinCountInfo,
                ServerOperationType.SetMyCemetery,
                ServerOperationType.SetEnemyCemetery,
                ServerOperationType.SetMyDeck,
                ServerOperationType.SetNameInfo
            };
            for (var id = 0; id < existing.Length; id++) Assert.Equal(id, (int)existing[id]);
            Assert.Equal(30, (int)ServerOperationType.SetCard);
            Assert.Equal(42, (int)ServerOperationType.SetMyDeck);
            Assert.Equal(43, (int)ServerOperationType.SetNameInfo);
            Assert.Equal(44, (int)ServerOperationType.SetEnemyDeck);
            Assert.Equal(45, System.Enum.GetValues<ServerOperationType>().Distinct().Count());
        }

        private static HeadlessGameFixture CreateEmptyDeckFixture()
        {
            var fixture = new HeadlessGameFixture();
            fixture.Game.PlayersDeck[0].Clear();
            fixture.Game.PlayersDeck[1].Clear();
            return fixture;
        }

        private static Viewer Record(HeadlessGameFixture fixture, List<Operation<ServerOperationType>> operations)
        {
            var viewer = new Viewer(new User("protocol-viewer", "headless"), () => null);
            viewer.Receive -= viewer.AddOperation;
            viewer.Receive += args =>
            {
                // Capture serialized values immediately, just as the transport does.
                operations.Add(JsonConvert.DeserializeObject<Operation<ServerOperationType>>(
                    JsonConvert.SerializeObject(args.Result)));
                return Task.CompletedTask;
            };
            fixture.Game.ViewList.Add(viewer);
            return viewer;
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task SnapshotAndSinglePlayerDeckRefreshUseTheSamePerspective(int owner)
        {
            var f = CreateEmptyDeckFixture();
            var first = f.AddCard(0, CardId.GeraltOfRivia, RowPosition.MyDeck);
            var second = f.AddCard(1, CardId.Dandelion, RowPosition.MyDeck);
            second.Status.IsPremium = true;
            var messages = new List<Operation<ServerOperationType>>();
            var viewer = Record(f, messages);
            await (Task)typeof(GwentServerGame).GetMethod("InitializeSpectator", BindingFlags.Instance | BindingFlags.NonPublic)
                .Invoke(f.Game, new object[] { viewer });
            AssertDeck(messages, ServerOperationType.SetMyDeck, first.Status.CardId, false);
            AssertDeck(messages, ServerOperationType.SetEnemyDeck, second.Status.CardId, true);
            messages.Clear();
            await f.Game.SetDeckInfo(owner);
            AssertDeck(messages, owner == 0 ? ServerOperationType.SetMyDeck : ServerOperationType.SetEnemyDeck,
                owner == 0 ? first.Status.CardId : second.Status.CardId, owner == 1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task DrawAndMulliganRestoreRealHandIdentityForEitherSide(int owner)
        {
            var f = CreateEmptyDeckFixture();
            var drawn = f.AddCard(owner, CardId.GeraltOfRivia, RowPosition.MyDeck);
            drawn.Status.IsPremium = true;
            var replacement = f.AddCard(owner, CardId.Dandelion, RowPosition.MyDeck);
            await f.SynchronizeClientsAsync();
            var messages = new List<Operation<ServerOperationType>>();
            Record(f, messages);
            await f.Game.DrawCard(owner == 0 ? 1 : 0, owner == 1 ? 1 : 0);
            AssertHandUpdate(messages, owner, drawn.Status.CardId, true);
            var opponent = owner == 0 ? f.SecondPlayer : f.FirstPlayer;
            Assert.True(opponent.Data.GetCard(new CardLocation(RowPosition.EnemyHand, 0)).IsCardBack);
            messages.Clear();
            (owner == 0 ? f.FirstPlayer : f.SecondPlayer).MulliganChoices.Enqueue(0);
            await f.Game.MulliganCard(owner, 1);
            AssertHandUpdate(messages, owner, replacement.Status.CardId, false);
            Assert.True(opponent.Data.GetCard(new CardLocation(RowPosition.EnemyHand, 0)).IsCardBack);
            AssertDeck(messages, owner == 0 ? ServerOperationType.SetMyDeck : ServerOperationType.SetEnemyDeck,
                drawn.Status.CardId, true);
        }

        [Fact]
        public async Task HiddenCardStatusUpdateUsesSpectatorPerspectiveAndSkipsUnindexedZones()
        {
            var f = CreateEmptyDeckFixture();
            var card = f.AddCard(1, CardId.GeraltOfRivia, RowPosition.MyHand);
            await f.SynchronizeClientsAsync();
            var messages = new List<Operation<ServerOperationType>>();
            Record(f, messages);
            await f.Game.ShowSetCard(card);
            AssertHandUpdate(messages, 1, card.Status.CardId, false);
            messages.Clear();
            await f.Game.LogicCardMove(card, f.Game.PlayersDeck[1], 0);
            messages.Clear();
            await f.Game.ShowSetCard(card);
            Assert.DoesNotContain(messages, op => op.OperationType == ServerOperationType.SetCard);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        public async Task SpectatorStatusDoesNotRevealConcealedCardsToPlayersOrChangeConceal(int owner)
        {
            var f = CreateEmptyDeckFixture();
            var card = f.AddCard(owner, CardId.GeraltOfRivia, RowPosition.MyRow1);
            card.Status.Conceal = true;
            await f.SynchronizeClientsAsync();
            var messages = new List<Operation<ServerOperationType>>();
            Record(f, messages);
            await f.Game.ShowSetCard(card);
            var actual = JsonConvert.DeserializeObject<CardStatus>(messages.Last().Arguments.Last());
            Assert.Equal(card.Status.CardId, actual.CardId);
            Assert.True(actual.Conceal);
            Assert.True(card.Status.Conceal);
            var opponent = owner == 0 ? f.SecondPlayer : f.FirstPlayer;
            Assert.True(opponent.Data.GetCard(new CardLocation(RowPosition.EnemyRow1, 0)).IsCardBack);
            var own = owner == 0 ? f.FirstPlayer : f.SecondPlayer;
            Assert.True(own.Data.GetCard(new CardLocation(RowPosition.MyRow1, 0)).IsCardBack);
        }

        private static void AssertDeck(List<Operation<ServerOperationType>> messages, ServerOperationType operation, string cardId, bool premium)
        {
            var cards = JsonConvert.DeserializeObject<List<CardStatus>>(messages.Last(op => op.OperationType == operation).Arguments.First());
            var card = Assert.Single(cards);
            Assert.Equal(cardId, card.CardId);
            Assert.Equal(premium, card.IsPremium == true);
        }

        private static void AssertHandUpdate(List<Operation<ServerOperationType>> messages, int owner, string cardId, bool premium)
        {
            var op = messages.Last(message => message.OperationType == ServerOperationType.SetCard);
            var location = JsonConvert.DeserializeObject<CardLocation>(op.Arguments.First());
            var card = JsonConvert.DeserializeObject<CardStatus>(op.Arguments.Last());
            Assert.Equal(owner == 0 ? RowPosition.MyHand : RowPosition.EnemyHand, location.RowPosition);
            Assert.Equal(0, location.CardIndex);
            Assert.Equal(cardId, card.CardId);
            Assert.False(card.IsCardBack);
            Assert.Equal(premium, card.IsPremium == true);
        }
    }
}
