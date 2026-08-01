using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33012")]//瑟瑞特
    public class Serrit : CardEffect
    {//对1个敌军单位造成7点伤害，或将对方1张被揭示的单位牌战力降为1点
        public Serrit(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, filter: x => x.Status.CardRow.IsInHand() ? x.Status.IsReveal : true, selectMode: SelectModeType.Enemy);
            if (cards.Count == 0) return 0;
            var targetCard = cards.Single();
            if (targetCard.Status.CardRow.IsInHand())
            {
                await targetCard.Effect.Damage(targetCard.CardPoint() - 1, Card);
            }
            else
            {
                await targetCard.Effect.Damage(7, Card);
            }

            return 0;
        }
    }
}