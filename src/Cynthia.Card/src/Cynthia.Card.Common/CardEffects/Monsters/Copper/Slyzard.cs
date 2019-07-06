using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24037")]//飞蜥
    public class Slyzard : CardEffect
    {
        //从己方墓场吞噬1个非同名铜色单位，并从牌组打出1张它的同名牌。
        public Slyzard(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //从墓地选牌
            var list = Game.PlayersCemetery[PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardId != Card.CardInfo().CardId);
            //筛选卡组还有同名卡的
            list = list.Where(x =>
                Game.PlayersDeck[Card.PlayerIndex].Any(y => y.CardInfo().CardId == x.CardInfo().CardId));

            // 没有候选卡就返回
            if (list.Count() == 0)
            {
                return 0;
            }

            //选卡
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1);
            if (result.Count() == 0)
            {
                return 0;
            }
            var target = result.Single();

            //吞噬
            await Card.Effect.Consume(target, x => 0);

            var cardToPlay = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.CardInfo().CardId == target.CardInfo().CardId);

            if (cardToPlay.Count() == 0)
            {
                return 0;
            }
            //打出新的同名牌
            await cardToPlay.Mess(Game.RNG).First().MoveToCardStayFirst();

            return 1;
        }
    }
}