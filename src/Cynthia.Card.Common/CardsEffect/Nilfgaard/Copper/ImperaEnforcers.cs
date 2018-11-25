using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
    [CardEffectId("64005")]//近卫军铁卫
    public class ImperaEnforcers : CardEffect
    {
        public ImperaEnforcers(IGwentServerGame game, GameCard card) : base(game, card) { }
        public int SpyingCount{get;set;} = 0;
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //选择对方场上一张卡,如果目标不为空,对其造成2点伤害
            var result = (await Game.GetSelectPlaceCards(1, Card,selectMode:SelectModeType.EnemyRow));
            if(result.Count!=0) await result.Single().Effect.Damage(2,Card);
            return 0;
        }
        public override async Task OnTurnOver(int playerIndex)
        {   //在回合结束,触发效果
            if(Card.PlayerIndex==playerIndex&&Card.Status.CardRow.IsOnPlace())
            {
                for(var i = 0; i<SpyingCount;i++)
                {   //重复计数次效果,之后清空计数
                    var result = (await Game.GetSelectPlaceCards(1, Card,selectMode:SelectModeType.EnemyRow));
                    if(result.Count!=0) await result.Single().Effect.Damage(2,Card);
                }
            }
            SpyingCount=0;
        }
        public override Task OnCardSpyingChange(GameCard taget, bool isSpying, GameCard soure = null)
        {   //当对方场上出现间谍,并且自己在场,计数+1
            if(taget.PlayerIndex!=Card.PlayerIndex&&isSpying&&Card.Status.CardRow.IsOnPlace())
                SpyingCount++;
            return Task.CompletedTask;
        }
    }
}