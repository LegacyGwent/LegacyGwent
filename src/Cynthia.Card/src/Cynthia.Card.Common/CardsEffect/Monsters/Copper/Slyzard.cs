using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24037")]//飞蜥
    public class Slyzard : CardEffect
    {
        //从己方墓场吞噬1个非同名铜色单位，并从牌组打出1张它的同名牌。
        public Slyzard(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //从墓地选牌
            var list = Game.PlayersCemetery[PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.Status.CardId != CardId.Slyzard).Mess();

            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1);
            if (result.Count() == 0) 
            {
                return 0;
            }

            //吞噬
            Consume(result.First());

            return 1;
        }


        //重写Consume
        public override async Task Consume(GameCard taget)
        {
            if (!Card.Status.CardRow.IsOnPlace() || taget.Status.CardRow == RowPosition.Banish) return;
            var num = taget.Status.Strength + taget.Status.HealthStatus;
            //被吞噬的目标
            if (taget.Status.CardRow.IsInCemetery())
            {//如果在墓地,放逐掉
                await taget.Effect.Banish();
            }
            else if (taget.Status.CardRow.IsOnRow())
            {//如果在场上,展示吞噬动画,之后不展示动画的情况进入墓地
                await taget.Effect.ToCemetery(CardBreakEffectType.Consume);
            }
            else
            {
                await taget.Effect.ToCemetery();
            }


            //将buff改为打出新的同名牌，其余的都没有变动
            //await Boost(num);
            var cardToPlay = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x.Status.CardId == target.Status.CardId).Mess();
                
            await cardToPlay.First().MoveToCardStayFirst();


            await Task.Delay(500);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //吞噬,应该触发对应事件<暂未定义,待补充>
            await Game.OnCardConsume(Card, taget);
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
    }
}