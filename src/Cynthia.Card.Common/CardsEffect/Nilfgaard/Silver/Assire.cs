using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33005")]//艾希蕾
    public class Assire : CardEffect
    {
        public Assire(IGwentServerGame game, GameCard card) : base(game, card){}

        public override async Task<int> CardPlayEffect(bool isSpying)
        {//将双方墓场中最多2张铜色/银色牌放回各自牌组。
            //墓地中所有的铜银色卡(先是我方墓地,再是敌方墓地)
            var list = Game.PlayersCemetery[Card.PlayerIndex]
            .Concat(Game.PlayersCemetery[Game.AnotherPlayer(Card.PlayerIndex)])
            .Where(x=>x.Status.Group==Group.Copper||x.Status.Group==Group.Silver).ToList();
            //让玩家选择
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list,2,isCanOver:true);
            result.ToList().ForAll(x=>
            {
                var playerIndex = x.PlayerIndex;
                Game.ShowCardMove(new CardLocation(RowPosition.MyDeck,new Random().Next(0,Game.PlayersDeck[playerIndex].Count)),x);
            });
            return 0;
        }
    }
}