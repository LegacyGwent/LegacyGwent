using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54027")] //多尔·布雷坦纳哨兵
    public class DolBlathannaSentry : CardEffect, IHandlesEvent<BeforeSpecialPlay>
    {
        //位于己方半场、牌组或手牌：己方打出特殊牌时获得1点增益。
        public DolBlathannaSentry(GameCard card) : base(card)
        {
        }

        private const int boost = 1;

        public async Task HandleEvent(BeforeSpecialPlay @event)
        {
            if (@event.Target.PlayerIndex == Card.PlayerIndex)
            {
                var position = Card.Status.CardRow;
                if (position.IsInDeck() || position.IsInHand() || position.IsOnPlace())
                {
                    await Card.Effect.Boost(boost, Card);
                }
            };
        }
    }
}