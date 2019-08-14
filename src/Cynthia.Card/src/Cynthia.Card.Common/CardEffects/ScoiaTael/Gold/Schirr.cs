using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("52010")] //希鲁
    public class Schirr : CardEffect
    {
        //生成“烧灼”或“瘟疫”。
        public Schirr(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return await Card.CreateAndMoveStay(CardId.Scorch, CardId.Epidemic);
        }
    }
}