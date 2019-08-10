using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44005")]//战地医师
    public class FieldMedic : CardEffect
    {//使友军“士兵”单位获得1点增益。
        public FieldMedic(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.GetPlaceCards(PlayerIndex).FilterCards(filter: x => x.HasAllCategorie(Categorie.Soldier));
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Boost(1, Card);
            }
            return 0;
        }
    }
}