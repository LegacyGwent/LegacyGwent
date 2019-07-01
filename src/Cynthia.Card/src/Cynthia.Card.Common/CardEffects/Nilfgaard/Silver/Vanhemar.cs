using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33015")]//凡赫玛
    public class Vanhemar : CardEffect
    {//生成“刺骨冰霜”、“晴空”或“复仇”。
        public Vanhemar(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            return await Card.CreateAndMoveStay(CardId.BitingFrost, CardId.ClearSkies, CardId.Shrike);
        }
    }
}