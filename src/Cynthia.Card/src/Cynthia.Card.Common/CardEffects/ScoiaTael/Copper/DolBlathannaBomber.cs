using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54014")] //多尔·布雷坦纳爆破手
    public class DolBlathannaBomber : CardEffect
    {
        //在对方单排生成1个“焚烧陷阱”。
        public DolBlathannaBomber(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            return await Card.CreateAndMoveStay(CardId.IncineratingTrap);
        }
    }
}