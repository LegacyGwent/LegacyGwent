using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("61002")]//克拉茨·奎特
    public class CrachAnCraite : CardEffect
    {//使牌组中最强的非间谍铜色/银色单位牌获得2点强化，随后打出。
        public CrachAnCraite(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //计算最强的时候算绿字
            // //卡组中所有最强非间谍铜色/银色单位牌
            // var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow && (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit)).WhereAllHighest();


            //计算最强的时候不算绿字
            //列出全部非间谍单位
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow && (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit));
            if (list.Count() == 0)
            {
                return 0;
            }
            //仿照WhereAllHighest,构造一个力量排序函数。
            var StrengthList = list.Select(x => (Strength: x.Status.Strength, card: x)).OrderByDescending(x => x.Strength);
            var StrengthMaximun = StrengthList.First().Strength;
            var result = StrengthList.Where(x => x.Strength >= StrengthMaximun).Select(x => x.card);
            //选一张
            if (!result.TryMessOne(out var target, Game.RNG))
            {
                return 0;
            }

            //强化并打出
            await target.Effect.Strengthen(2, Card);
            await target.MoveToCardStayFirst();
            return 1;

        }
    }
}