using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("13001")]//萝卜
    public class Roach : CardEffect
    {
        public Roach(IGwentServerGame game, GameCard card) : base(game, card) { }
        // public override async Task OnUnitPlay(GameCard taget)
        // {
        //     if (taget.Status.Group == Group.Gold && taget.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
        //     {
        //         await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex), taget);
        //     }
        // }
        public override async Task OnUnitDown(GameCard taget)
        {
            if (taget.Status.Group == Group.Gold && taget.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsInDeck())
            {
                await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex), taget);
            }
        }
    }
}