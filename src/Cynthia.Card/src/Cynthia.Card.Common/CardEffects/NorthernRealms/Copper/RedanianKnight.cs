using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44012")]//瑞达尼亚骑士
    public class RedanianKnight : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，若不受护甲保护，则获得2点增益和2点护甲。
        public RedanianKnight(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (Card.Status.CardRow.IsOnPlace() && Card.Status.Armor <= 0)
            {
                await Card.Effect.Armor(2, Card);
                await Card.Effect.Boost(2, Card);
            }
            return;

        }

    }