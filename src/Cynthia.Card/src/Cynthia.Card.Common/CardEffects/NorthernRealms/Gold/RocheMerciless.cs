using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("42007")]//罗契：冷酷之心
    public class RocheMerciless : CardEffect
    {//摧毁1个背面向上的伏击敌军单位。
        public RocheMerciless(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var switchCard = await Card.GetMenuSwitch(("", "打出1张与自身战力相同的银色/铜色泰莫利亚牌。。"), ("", "摧毁1个背面向上的伏击敌军单位。"));
            if (switchCard == 0)
            {
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Temeria) && x.CardPoint() == Card.CardPoint() &&
                       (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper)).Mess(Game.RNG).ToList();

                if (list.Count() == 0)
                {
                    return 0;
                }

                var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 1);
                if (cards.Count() == 0)
                {
                    return 0;
                }

                //打出
                var playCard = cards.Single();
                await playCard.MoveToCardStayFirst();
                return 1;
            }
            if (switchCard == 1)
            {
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow, filter: x => x.Status.Conceal == true);
                if (!selectList.TrySingle(out var target))
                {
                    return 0;
                }
                await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
                return 0;
            }
            return 0;
           
        }
    }
}
