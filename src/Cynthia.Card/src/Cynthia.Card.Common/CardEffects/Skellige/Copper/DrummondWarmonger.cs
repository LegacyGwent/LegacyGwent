using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64011")]//德拉蒙家族好战分子
    public class DrummondWarmonger : CardEffect
    {//从牌组丢弃1张铜色牌。
        public DrummondWarmonger(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.Group == Group.Copper).Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "选择丢弃一张牌");

            if (result.Count() == 0)
            {
                return 0;
            }
            await result.First().Effect.Discard(Card);

            return 0;
        }
    }
}