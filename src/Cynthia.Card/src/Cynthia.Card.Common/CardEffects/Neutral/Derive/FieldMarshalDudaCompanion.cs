using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("15007")]//话篓子：伙伴
    public class FieldMarshalDudaCompanion : CardEffect
    {//使左右各2格内的单位获得2点增益。
        public FieldMarshalDudaCompanion(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Card.GetRangeCard(2, type: GetRangeType.HollowAll).ToList();
            if (list.Count() == 0)
            {
                return 0;
            }
            foreach (var card in list)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    await card.Effect.Boost(2, Card);
                }
            }
            return 0;
        }
    }
}