using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24038")]//月光
    public class Moonlight : CardEffect
    {//择一：降下“满月”恩泽；或降下“血月”灾厄。
        public Moonlight(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            return await Card.CreateAndMoveStay(CardId.FullMoon, CardId.BloodMoon);
        }
    }
}