using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130050")]//吉尔曼：晋升
    public class GermainPiquantPro : CardEffect
    {//在两侧各生成3头“牛”。
        public GermainPiquantPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            if (!Card.Status.CardRow.IsOnPlace()) return 0;
            await Game.CreateCard(CardId.Cow, PlayerIndex, Card.GetLocation());
            await Game.CreateCard(CardId.Cow, PlayerIndex, Card.GetLocation());
            await Game.CreateCard(CardId.Cow, PlayerIndex, Card.GetLocation());
            await Game.CreateCard(CardId.Cow, PlayerIndex, Card.GetLocation().With(x => x.CardIndex++));
            await Game.CreateCard(CardId.Cow, PlayerIndex, Card.GetLocation().With(x => x.CardIndex++));
            await Game.CreateCard(CardId.Cow, PlayerIndex, Card.GetLocation().With(x => x.CardIndex++));
            return 0;
        }
    }
}