using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("13001")]//萝卜
    public class Roach : CardEffect
    {
        public Roach(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task OnUnitPlay(GameCard taget)
        {
            if (taget.Status.Group == Group.Gold && taget.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
            {
                var a = new List<int>();
            }
        }
    }
}