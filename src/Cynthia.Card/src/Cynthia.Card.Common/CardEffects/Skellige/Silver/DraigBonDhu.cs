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
            var mylist = Game.PlayersCemetery[Card.PlayerIndex].Where(x => x.CardInfo().CardType == CardType.Unit).ToList();
            //显示敌方墓地单位卡(顺序)
            var enemylist = Game.PlayersCemetery[Game.AnotherPlayer(Card.PlayerIndex)].Where(x => x.CardInfo().CardType == CardType.Unit).ToList();
            var list = new List<GameCard>();
            //合并
            list = mylist.Concat(enemylist).ToList();
            //如果双方墓地都没有单位，什么都不做
            if (list.Count() == 0)
            {
                return 0;
            }
            //选择两张牌，如果不选，什么都不做
            //不确定的提示语 var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2,"选择强化两张牌");

            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2);
            if (result.Count() == 0)
            {
                return 0;
            }
            //强化
            foreach (var card in result)
            {
                await card.Effect.Strengthen(3, Card);
            }
            return 1;
        }
    }
}