using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70081")] //德拉蒙德-皮勒格
    public class DrummondPillager : CardEffect, IHandlesEvent<AfterCardDiscard>
    {
        //when discarded, strengthen all drummond units by 1 wherever they are.
        public DrummondPillager(GameCard card) : base(card)
        {
        }

        private async Task StrengthenAllDrummond(int Strengthenpoint)
        {
            var targets = Game.PlayersHandCard[PlayerIndex].Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.HasAllCategorie(Categorie.ClanDrummond) && x != Card);            foreach (var target in targets)
            {
                await target.Effect.Strengthen(1, Card);
            }
        }

        private const int Strengthenpoint = 1;
        
        public async Task HandleEvent(AfterCardDiscard @event)
        {
            if (@event.Target != Card)
            {
                return;
            }

            await StrengthenAllDrummond(Strengthenpoint);
        }
    }
}
