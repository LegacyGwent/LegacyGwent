using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70146")]//加尔 Gael
    public class Gael : CardEffect
    {//
        public Gael(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
           var targets = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.AllRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            foreach (var target in targets)
            {
                await target.Effect.Damage(5, Card);
            }

            return 0;
        }
    }
}

