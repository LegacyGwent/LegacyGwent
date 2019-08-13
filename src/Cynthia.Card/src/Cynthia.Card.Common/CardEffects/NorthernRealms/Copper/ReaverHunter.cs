using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44021")]//掠夺者猎人
    public class ReaverHunter : CardEffect, IHandlesEvent<AfterUnitPlay>
    {//使手牌、牌组或己方半场所有同名牌获得1点增益。 每有1张同名牌打出，便再次触发此能力。
        public ReaverHunter(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex]).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.Status.CardId == Card.Status.CardId);
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Boost(1, Card);
            }
            return 0;
        }
        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.Status.CardId == Card.Status.CardId && Card.Status.CardRow.IsOnPlace() && Card.PlayerIndex == @event.PlayedCard.PlayerIndex && @event.PlayedCard != Card)
            {
                var cards = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex]).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.Status.CardId == Card.Status.CardId);
                if (cards.Count() == 0)
                {
                    return;
                }
                foreach (var card in cards)
                {
                    await card.Effect.Boost(1, Card);
                }

            }
            return;
        }

    }
}