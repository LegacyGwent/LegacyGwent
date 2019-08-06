using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64030")]//海玫家族保卫者
    public class HeymaeyProtector : CardEffect
    {//从牌组打出1张铜色“道具”牌。
        public HeymaeyProtector(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //乱序列出卡组中铜色道具卡
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Categories.Contains(Categorie.Item) &&
                     x.Status.Group == Group.Copper)
                  .Mess(Game.RNG)
                  .ToList();

            if (list.Count() == 0)
            {
                return 0;
            }
            //选一张，如果没选，什么都不做
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
    }
}