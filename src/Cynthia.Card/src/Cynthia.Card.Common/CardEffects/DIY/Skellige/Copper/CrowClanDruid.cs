using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70134")]//鸦母德鲁伊 CrowClanDruid
    public class CrowClanDruid : CardEffect, IHandlesEvent<AfterTurnOver>
    {//
        public CrowClanDruid(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var crowlist = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && x.Status.CardId == CardId.Crow).ToList();;
            var rowlist = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow);
            if(crowlist.Count() == 0 && rowlist.Count < Game.RowMaxCount)
            {
                await Game.CreateCard(CardId.Crow, PlayerIndex, Card.GetLocation()+1);
            }
        }
    }
}
