using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70083")]//红骑士
    public class RedRider : CardEffect, IHandlesEvent<BeforeCardToCemetery>
    {// When an enemy unit is moved to the graveyard from a row with Biting Frost, if this card is in your deck, summon it on a random row.
        public bool IsToSummon = false;
        public RedRider(GameCard card) : base(card) { }
        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            if (@event.Target.Status.Type == CardType.Unit && @event.Target.PlayerIndex == AnotherPlayer && Card.Status.CardRow.IsInDeck())
            {
                if (!@event.DeathLocation.RowPosition.IsOnPlace())
                {
                    return;
                }

                bool isInFrost = Game.GameRowEffect[@event.Target.PlayerIndex][@event.DeathLocation.RowPosition.MyRowToIndex()].RowStatus == RowStatus.BitingFrost;
                if (isInFrost)
                {
                    var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId && (x.Effect as RedRider)?.IsToSummon == false).ToList();
                    if (list.Count() == 0)
                    {
                        return;
                    }
                    //只召唤最后一个
                    if (Card == list.Last())
                    {
                        await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), list.First());
                    }
                }
            }
        }

    }
}