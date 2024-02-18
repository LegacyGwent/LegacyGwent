using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70143")]//辛特拉骑士 
    public class CintrianKnight : CardEffect, IHandlesEvent<AfterUnitPlay>
    {//己方打出辛特拉单位时，使自身与打出的辛特拉单位获得1点增益与1点护甲。
        public CintrianKnight(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.PlayedCard != Card )
            {
                if (@event.PlayedCard.HasAnyCategorie(Categorie.Cintra))
                {
                    await Boost(1, Card);
                    await @event.PlayedCard.Effect.Boost(1, Card);
                }
            }
            return;
        }
    }
}
