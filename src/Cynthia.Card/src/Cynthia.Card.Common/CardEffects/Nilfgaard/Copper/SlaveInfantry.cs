using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34027")]//奴隶步兵
    public class SlaveInfantry : CardEffect
    {//在己方其他排生成1张佚亡原始同名牌。
        public SlaveInfantry(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            for (var i = 0; i < 3; i++)
            {
                if (Card.Status.CardRow == i.IndexToMyRow()) continue;
                await Game.CreateCardAtEnd("34027", Card.PlayerIndex, i.IndexToMyRow(), x => x.IsDoomed = true);
            }
            return 0;
        }
    }
}