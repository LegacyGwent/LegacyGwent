using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54011")] //私枭后援者
    public class HawkerSupport : CardEffect
    {
        //使手牌中1张单位牌获得3点增益。
        public HawkerSupport(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = Game.PlayersHandCard[Card.PlayerIndex].Where(x=>(x.Status.Type == CardType.Unit));
            var list = cards.ToList();
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
                (Card.PlayerIndex, list.ToList(), 1, "选择增益一张牌");
            //如果玩家一张卡都没选择,没有效果
            if (!result.Any()) return 0;
            var card = result.Single();
            await card.Effect.Boost(boost,Card);
            return 0;
        }

        private const int boost = 3;
    }
}