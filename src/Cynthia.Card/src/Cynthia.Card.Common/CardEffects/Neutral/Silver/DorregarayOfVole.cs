using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13020")]//多瑞加雷
    public class DorregarayOfVole : CardEffect
    {//生成1只“恶熊”、“翼手龙”、“须岩怪”或“水鬼”。
        public DorregarayOfVole(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return await Card.CreateAndMoveStay(
                CardId.SavageBear,
                CardId.Wyvern,
                CardId.Barbegazi,
                CardId.Drowner);
        }
    }
}
