using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Cynthia.Card.AI;
using Cynthia.Card.Server;

namespace Cynthia.Card.Gameplay.Tests
{
    internal sealed class HeadlessGameFixture
    {
        public HeadlessGameFixture()
        {
            FirstPlayer = new DeterministicHeadlessPlayer("headless-first");
            SecondPlayer = new DeterministicHeadlessPlayer("headless-second");
            Game = new GwentServerGame(FirstPlayer, SecondPlayer);
        }

        public DeterministicHeadlessPlayer FirstPlayer { get; }
        public DeterministicHeadlessPlayer SecondPlayer { get; }
        public GwentServerGame Game { get; }

        public GameCard AddCard(int playerIndex, string cardId, RowPosition row, int? strength = null)
        {
            var status = new CardStatus(cardId, Game.PlayersFaction[playerIndex], row);
            if (strength.HasValue)
            {
                status.Strength = strength.Value;
            }

            var card = new GameCard(Game, playerIndex, status, cardId);
            Game.RowToList(playerIndex, row).Add(card);
            return card;
        }

        public static void ReplaceMainEffect(GameCard card, CardEffect effect)
        {
            card.Effects.Clear();
            card.Effects.Add(effect);
            card.Effect = effect;
        }

        public Task SynchronizeClientsAsync() => Game.SetAllInfo();
    }

    internal sealed class DeterministicHeadlessPlayer : RandomAutoAIPlayer
    {
        private readonly string _requestedName;
        private readonly Queue<string> _queuedMenuCardIds = new Queue<string>();

        public DeterministicHeadlessPlayer(string playerName)
        {
            _requestedName = playerName;
            PlayerName = playerName;
        }

        public override void SetDeckAndName()
        {
            PlayerName = _requestedName ?? "headless";
            Deck = GwentDeck.CreateBasicDeck(0);
        }

        public override void SelectMenuCards(MenuSelectCardInfo info, Action<Operation<UserOperationType>> send)
        {
            LastMenuOptionCount = info.SelectList.Count;
            var selected = new List<int>();
            while (selected.Count < info.SelectCount && _queuedMenuCardIds.Count > 0)
            {
                var requestedCardId = _queuedMenuCardIds.Dequeue();
                var index = Enumerable.Range(0, info.SelectList.Count)
                    .FirstOrDefault(candidate =>
                        !selected.Contains(candidate) &&
                        info.SelectList[candidate].CardId == requestedCardId,
                        -1);
                if (index >= 0)
                {
                    selected.Add(index);
                }
            }
            selected.AddRange(Enumerable.Range(0, info.SelectList.Count)
                .Where(index => !selected.Contains(index))
                .Take(info.SelectCount - selected.Count));
            send(Operation.Create(UserOperationType.SelectMenuCardsInfo, selected));
        }

        public void QueueMenuCardIds(params string[] cardIds)
        {
            foreach (var cardId in cardIds)
            {
                _queuedMenuCardIds.Enqueue(cardId);
            }
        }

        public int LastMenuOptionCount { get; private set; }

        public IList<CardLocation> PlaceSelectionSources { get; } = new List<CardLocation>();

        public override void SelectPlaceCards(PlaceSelectCardsInfo info, Action<Operation<UserOperationType>> send)
        {
            PlaceSelectionSources.Add(new CardLocation(
                info.SelectCard.RowPosition,
                info.SelectCard.CardIndex));
            var selected = info.CanSelect.CardsPartToLocation().Take(info.SelectCount).ToList();
            send(Operation.Create(UserOperationType.SelectPlaceCardsInfo, selected));
        }

        public override void SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.SelectRowInfo, rowPart.First()));
        }
    }
}
