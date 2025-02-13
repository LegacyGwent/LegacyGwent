using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23005")]//伊夫利特
    public class Ifrit : CardEffect
    {//在右侧生成3个“次级伊夫利特”。
        public Ifrit(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 4; i++)
            {
                await Game.CreateCard(CardId.LesserIfrit, PlayerIndex, Card.GetLocation().With(x => x.CardIndex++));
            }
            return 0;
        }
    }
}