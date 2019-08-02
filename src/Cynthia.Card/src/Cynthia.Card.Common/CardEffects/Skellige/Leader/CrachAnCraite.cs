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
            //卡组中所有最强非间谍铜色/银色单位牌
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.isSpying() = false && (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit)).WhereAllHighest();
            //选一张
            if (!list.TryMessOne(out var target, Game.RNG))
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