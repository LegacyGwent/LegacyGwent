using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("32006")]//威戈佛特兹
    public class Vilgefortz : CardEffect
    {
        public Vilgefortz(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //摧毁1个友军单位，随后从牌组顶端打出1张牌；或
            //休战：摧毁1个敌军单位，随后对方抽1张铜色牌并揭示它。
            var result = default(IList<GameCard>);//如果休战,只能选择己方卡牌
            if (Game.IsPlayersPass.Any(x => x))
                result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow));
            else result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow));
            if (result.Count != 0)
            {
                var tagetCard = result.Single();
                await tagetCard.Effect.ToCemetery(CardBreakEffectType.Scorch);
                if (tagetCard.PlayerIndex == Card.PlayerIndex)//我方
                {   //如果我方卡组没有卡牌...直接返回
                    if (Game.PlayersDeck[Card.PlayerIndex].Count <= 0) return 0;
                    await Game.PlayersDeck[Card.PlayerIndex].ToList()[0].MoveToCardStayFirst();
                    return 1;
                }
                else//敌方
                {
                    var drawCards = await Game.PlayerDrawCard(Game.AnotherPlayer(Card.PlayerIndex), 1, x => x.Status.Group == Group.Copper);
                    if (drawCards.Any())
                        await drawCards.Single().Effect.Reveal(Card);
                }
            }
            return 0;
        }
    }
}