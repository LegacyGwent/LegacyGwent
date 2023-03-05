using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70111")]//融雪 Thaw
    public class Thaw : CardEffect
    {//随机使1个友军单位获得3点增益。本回合中每打出过1张牌便重复1次。
        public Thaw(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            for(int s = 0; s <= Game.TurnCardPlayedNum; s++)
            {
                var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == Card.PlayerIndex).Mess(RNG).Take(1).ToList();
                foreach (var card in cards)
                {
                    await card.Effect.Boost(3, Card);
                }
            }
            return 0;
        }
    }
}