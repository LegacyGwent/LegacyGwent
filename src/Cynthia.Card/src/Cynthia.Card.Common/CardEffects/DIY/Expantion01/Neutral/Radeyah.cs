using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70072")]//雷蒂娅 Radeyah
    public class Radeyah: CardEffect
    {//造成等同于手牌中立牌数量的伤害，并将手牌和牌库中2张核心系列中立银色单位牌变为金色晋升牌。
        public Radeyah(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = Game.PlayersHandCard[Card.PlayerIndex].Where(x=>(x.Status.Faction == Faction.Neutral));
            
            var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count > 0)
            {
                await result.Single().Effect.Damage(cards.Count(),Card);
            }
			
            cards = Game.PlayersHandCard[Card.PlayerIndex].Where(x => int.Parse(x.Status.CardId)>= 13001 && int.Parse(x.Status.CardId) <= 13022);
            var list = cards.ToList();
            list = list.Concat(Game.PlayersDeck[Card.PlayerIndex].Where(x => int.Parse(x.Status.CardId)>= 13001 && int.Parse(x.Status.CardId) <= 13022)
                .Mess(Game.RNG).ToList()).ToList();
            //让玩家选择一张卡
            var result2 = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 2, "选择晋升2张牌");
            //如果玩家一张卡都没选择,没有效果
            if (!result2.Any()) return 0;
            foreach( var x in result2)
            {
                await x.Effect.Transform(x.CardInfo().CardId+"0", Card, isForce:true);
            }
            return 0;
        }
    }
}
