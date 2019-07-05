using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24037")]//飞蜥
    public class Slyzard : CardEffect
    {
        //从己方墓场吞噬1个非同名铜色单位，并从牌组打出1张它的同名牌。
        public Slyzard(GameCard card) : base(card){ }

        private  int ConsumeSuccess = 1;

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //从墓地选牌
            var list = Game.PlayersCemetery[PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.Status.CardId != Card.Status.CardId);
            //筛选卡组还有同名卡的
            list = list.Where(x => 
                Game.PlayersDeck[Card.PlayerIndex].Any(y => y.Status.CardId == x.Status.CardId));
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

            //吞噬
            Card.Effect.Consume(result.Single());

            return ConsumeSuccess;
        }

        //重写Consume
        public override async Task Consume(GameCard taget)
        {
            if (!Card.Status.CardRow.IsOnPlace() || target.Status.CardRow == RowPosition.Banish)
            {
                return;
            }
            //放逐目标
            await target.Effect.Banish();

            await Game.ClientDelay(500);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //吞噬,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardConsume(target, Card));
            //8888888888888888888888888888888888888888888888888888888888888888888888

            //将buff改为打出新的同名牌
            var cardToPlay = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x.Status.CardId == target.Status.CardId);
            if (cardToPlay.Count() == 0) 
            {
                ConsumeSuccess = 0;
                return;
            }
            //打出新的同名牌
            await cardToPlay.First().MoveToCardStayFirst();
        }
    }
}