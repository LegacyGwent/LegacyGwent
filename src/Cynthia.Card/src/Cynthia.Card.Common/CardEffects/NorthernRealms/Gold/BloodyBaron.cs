using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42002")]//血红男爵
    public class BloodyBaron : CardEffect, IHandlesEvent<AfterCardDeath>
    {//若位于手牌、牌组或己方半场：有敌军单位被摧毁时获得1点增益。
        public BloodyBaron(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target.PlayerIndex != Card.PlayerIndex && (Card.Status.CardRow.IsOnPlace() || Card.Status.CardRow.IsInDeck() || Card.Status.CardRow.IsInHand()))
            {
                await Card.Effect.Boost(1, Card);
            }
            return;
        }

    }
}