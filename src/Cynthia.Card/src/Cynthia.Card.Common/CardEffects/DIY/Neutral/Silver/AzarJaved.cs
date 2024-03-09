using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70161")]//阿扎·贾维德 AzarJaved
    public class AzarJaved : CardEffect
    {//
        public AzarJaved(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 2; i++)
            {
                await Game.CreateCard(CardId.Scarab, PlayerIndex,new CardLocation(RowPosition.MyStay, 0));
            }
            return 2;
        }
    }
}
