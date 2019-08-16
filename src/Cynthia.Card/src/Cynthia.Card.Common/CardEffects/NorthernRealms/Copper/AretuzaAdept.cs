using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44026")]//艾瑞图萨学院学员
    public class AretuzaAdept : CardEffect
    {//从牌组随机打出1张铜色灾厄牌。
        public AretuzaAdept(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //随机取出卡组中一张目标卡
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.HasAnyCategorie(Categorie.Hazard) &&
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