using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130200")]//多瑞加雷：晋升
    public class DorregarayOfVolePro : CardEffect
    {//生成 1 张“恶熊”、“翼手龙”、“须岩怪”或“水鬼”的银色晋升牌。
        public DorregarayOfVolePro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return await Card.CreateAndMoveStay(CardId.SavageBear+"0", CardId.Wyvern+"0", CardId.Barbegazi+"0", CardId.Drowner+"0");
        }
    }
}