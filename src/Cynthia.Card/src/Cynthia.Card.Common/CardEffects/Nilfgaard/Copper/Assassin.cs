using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34029")]//刺客
    public class Assassin : CardEffect
    {//间谍对左侧单位造成10点伤害。
        public Assassin(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var rowIndex = Card.GetLocation(Card.PlayerIndex).CardIndex;
            var list = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).ToList();
            var target = Card.GetRangeCard(1, GetRangeType.HollowLeft);
            if (target.Count > 0)
            {
                await target.Single().Effect.Damage(10, Card);
            }
            return 0;
        }
    }
}