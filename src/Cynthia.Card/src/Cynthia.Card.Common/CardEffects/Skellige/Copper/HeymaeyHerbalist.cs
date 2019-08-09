using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64027")]//海玫家族草药医生
    public class HeymaeyHerbalist : CardEffect
    {//从牌组打出1张随机铜色“有机”或灾厄牌。
        public HeymaeyHerbalist(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //随机取出卡组中一张目标卡
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.HasAnyCategorie(Categorie.Organic, Categorie.Hazard) &&
                     x.Status.Group == Group.Copper)
                  .Mess(Game.RNG)
                  .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }

            //打出
            await list.First().MoveToCardStayFirst();
            return 1;
        }
    }
}