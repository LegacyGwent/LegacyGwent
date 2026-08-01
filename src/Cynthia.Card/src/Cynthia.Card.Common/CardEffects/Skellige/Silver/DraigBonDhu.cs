using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63010")]//德莱格·波·德乌
    public class DraigBonDhu : CardEffect
    {//使墓场中2个单位获得3点强化。
        public DraigBonDhu(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于此卡可以选择双方墓地
            //显示我方墓地单位卡(顺序)
            var list = Game.PlayersCemetery[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit).ToList();

            //选择两张牌，如果不选，什么都不做
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2, "选择强化两张牌");

            foreach (var card in result)
            {
                await card.Effect.Strengthen(3, Card);
            }
            return 0;

        }
    }
}