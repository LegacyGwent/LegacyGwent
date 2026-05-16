using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70189")]//奥术法典 Arcane Tome
    public class ArcaneTome : CardEffect
    {//Damage a unit by 3. If it is destroyed, spawn and play a Kaedweni Revenant.
        public ArcaneTome(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!cards.TrySingle(out var target))
            {
                return 0;
            }
            var damagePoint = 3;
            await target.Effect.Damage(damagePoint, Card);      
            if (target.CardPoint() > 0)
            {
                return 0;
            }
            return await Card.CreateAndMoveStay(CardId.KaedweniRevenant);
        }
    }
}