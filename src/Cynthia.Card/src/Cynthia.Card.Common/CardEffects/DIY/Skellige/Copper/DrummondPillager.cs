using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70081")] //德拉蒙德-皮勒格
    public class DrummondPillager : CardEffect, IHandlesEvent<AfterUnitDown>
    {
        //when discarded, strengthen all drummond units by 1 wherever they are.
        public DrummondPillager(GameCard card) : base(card)
        {
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (!Card.Status.CardRow.IsOnPlace() ||
                @event.Target == Card ||
                @event.Target.PlayerIndex != PlayerIndex ||
                !@event.Target.HasAllCategorie(Categorie.ClanDrummond))
            {
                return;
            }

            await @event.Target.Effect.Strengthen(1, Card);
        }
    }
}
