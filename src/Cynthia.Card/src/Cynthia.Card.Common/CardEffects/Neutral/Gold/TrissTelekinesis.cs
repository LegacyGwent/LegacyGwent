using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12026")]//特莉丝：心灵传动
    public class TrissTelekinesis : CardEffect
    {//创造任意方起始牌组中的1张铜色特殊牌。
        public TrissTelekinesis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Card.GetBaseDeck(
                x => x.Group == Group.Copper &&
                x.CardType == CardType.Special,
                true
            );
            return await Card.CreateAndMoveStay(list.Select(x => x.CardId).Mess(RNG).Take(3).ToArray());
        }
    }
}