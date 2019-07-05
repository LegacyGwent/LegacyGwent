using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24031")]//孽鬼
    public class Nekker : CardEffect, IHandlesEvent<AfterCardDeath>, IHandlesEvent<AfterCardConsume>
    {//若位于手牌、牌组或己方半场：友军单位发动吞噬时获得1点增益。 遗愿：召唤1张同名牌。
        public Nekker(GameCard card) : base(card){ }
        
        //当友军单位发动吞噬时,并且自己在场位于手牌、牌组或己方半场时获得1点增益
        private const int boost = 1;

        public async Task HandleEvent(AfterCardConsume @event)
        {
            if(Card.PlayerIndex == @event.Source.PlayerIndex)
            {
                var position = Card.Status.CardRow;
                
                if (position.IsInDeck() || position.IsInHand() || position.IsOnPlace())
                {
                    await Card.Effect.Boost(boost, Card);
                }
            }
        }

        //遗愿：召唤1张同名牌
        public async Task HandleEvent(AfterCardDeath @event)
        {
            //如果不是自己死亡，返回
            if(@event.Taget!=Card)
            {    
                return;
            }

            //在自己的牌库中寻找同名卡
            var targets = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.CardId = Card.Status.CardId).Mess(Game.RNG);

            //如果没有，直接返回
            if (targets.Count() == 0) 
            {
                return;
            }
            
            //放置死亡的位置到上面
            await targets.First().Effect.Summon(@event.DeathLocation);
            return;
        }
    }
}