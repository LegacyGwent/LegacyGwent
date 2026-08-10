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
            var selected = Enumerable.Range(0, info.SelectList.Count).Take(info.SelectCount).ToList();
            send(Operation.Create(UserOperationType.SelectMenuCardsInfo, selected));
        }

        public int LastMenuOptionCount { get; private set; }

        public override void SelectPlaceCards(PlaceSelectCardsInfo info, Action<Operation<UserOperationType>> send)
        {
            var selected = info.CanSelect.CardsPartToLocation().Take(info.SelectCount).ToList();
            send(Operation.Create(UserOperationType.SelectPlaceCardsInfo, selected));
        }

        public override void SelectRow(CardLocation selectCard, IList<RowPosition> rowPart, Action<Operation<UserOperationType>> send)
        {
            send(Operation.Create(UserOperationType.SelectRowInfo, rowPart.First()));
        }
    }
}
