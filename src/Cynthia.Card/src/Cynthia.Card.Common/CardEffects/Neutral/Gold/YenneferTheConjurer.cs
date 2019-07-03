using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12010")]//叶奈法：咒术师
    public class YenneferTheConjurer : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，对所有最强的敌军单位造成1点伤害。
        public YenneferTheConjurer(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && PlayerIndex == @event.PlayerIndex)) return;
            var cards = Game.GetAllCard(Game.AnotherPlayer(Card.PlayerIndex))
                .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == AnotherPlayer)
                .WhereAllHighest().ToList();
            foreach (var card in cards)
            {
                await card.Effect.Damage(1, Card, BulletType.Lightnint);
            }
            return;
        }
    }
}