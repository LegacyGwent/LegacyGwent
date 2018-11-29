using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("32006")]//威戈佛特兹
    public class Vilgefortz : CardEffect
    {
        public Vilgefortz(IGwentServerGame game, GameCard card) : base(game, card){}
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //摧毁1个友军单位，随后从牌组顶端打出1张牌；或
            //休战：摧毁1个敌军单位，随后对方抽1张铜色牌并揭示它。
            var result = default(IList<GameCard>);//如果休战,只能选择己方卡牌
            if(Game.IsPlayersPass[Game.Player1Index]||Game.IsPlayersPass[Game.Player2Index])
                result = (await Game.GetSelectPlaceCards(1, Card,selectMode:SelectModeType.MyRow));
            else result = (await Game.GetSelectPlaceCards(1, Card,selectMode:SelectModeType.AllRow));
            if(result.Count!=0)
            {
                var tagetCard = result.Single();
                await tagetCard.Effect.ToCemetery(CardBreakEffectType.Scorch);
                if(tagetCard.PlayerIndex==Card.PlayerIndex)//我方
                {   //如果我方卡组没有卡牌...直接返回
                    if(Game.PlayersDeck[Card.PlayerIndex].Count<=0)return 0;
                    await Game.PlayersDeck[Card.PlayerIndex].ToList()[0].MoveToCardStayFirst();
                    return 1;
                }
                else//敌方
                {
                    if(!Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)].Any(x=>x.Status.Group==Group.Copper)) return 1;
                    var mCard = Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)].ToList().First(x=>x.Status.Group==Group.Copper);
                    await Game.LogicCardMove(mCard,Game.PlayersDeck[Game.AnotherPlayer(Card.PlayerIndex)],0);
                    if(Card.PlayerIndex==Game.Player1Index)//如果我是玩家1
                        await Game.DrawCard(0,1);
                    else await Game.DrawCard(1,0);
                    return 1;
                }
            }
            return 0;
        }
    }
}