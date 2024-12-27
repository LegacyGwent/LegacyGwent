using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54003")] //维里赫德旅新兵
    public class VriheddNeophyte : CardEffect
    {
        //随机使手牌中2个单位获得1点增益。
        public VriheddNeophyte(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersHandCard[Card.PlayerIndex];
            var list = cards.Where(x => x.CardInfo().CardType == CardType.Unit && x.CardInfo().CardUseInfo == CardUseInfo.MyRow).Mess(RNG).ToList().Take(2);
            if (!list.Any()) return 0;

            foreach (var it in list)
            {
                await it.Effect.Boost(boost, Card);
            }

            return 0;
        }

        private const int boost = 1;
    }
}