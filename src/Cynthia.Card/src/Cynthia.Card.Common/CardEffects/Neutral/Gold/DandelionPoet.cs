using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("12003")]//丹德里恩:传奇诗人
    public class DandelionPoet : CardEffect
    {
        public DandelionPoet(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var dcard = default(List<GameCard>);
            if (Card.PlayerIndex == Game.Player1Index)
                (dcard, _) = await Game.DrawCard(1, 0);
            else if (Card.PlayerIndex == Game.Player2Index)
                (_, dcard) = await Game.DrawCard(0, 1);
            if (dcard.Count == 0)
                return 0;
            else
            {
                var list = Game.PlayersHandCard[Card.PlayerIndex];
                //让玩家选择一张卡
                var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, isCanOver: false);
                //如果玩家一张卡都没选择,没有效果
                if (result.Count() == 0) return 0;
                await result.Single().MoveToCardStayFirst();
                return 1;
            }
        }
    }
}