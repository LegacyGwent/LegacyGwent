using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70147")] //渴血鸟怪 Plumard
    public class Plumard : CardEffect,IHandlesEvent<AfterCardDrain>, IHandlesEvent<AfterUnitPlay>, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardToCemetery>
    {
        public Plumard(GameCard card) : base(card){}
        private int vr = 0;
        public async Task HandleEvent(AfterCardDrain @event)
        {
            if (@event.Target.HasAnyCategorie(Categorie.Vampire))
            {
                vr = 1;
            }
            if (@event.Target.Status.CardId != Card.Status.CardId && @event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.Target.Status.CardRow == Card.Status.CardRow)
            {
                await Card.Effect.Drain(1, @event.Source);
            }
            
            return;
        }

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            if (@event.PlayedCard.PlayerIndex == Card.PlayerIndex && @event.PlayedCard != Card && @event.PlayedCard.HasAnyCategorie(Categorie.Vampire))
            {
                vr = 1;
            }
            await Task.CompletedTask;
            return;

        }

        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            if (@event.PlayerIndex != PlayerIndex)
            {
                return;
            }
            if ((vr == 1) && @event.Target.CardInfo().CardType == CardType.Unit && !@event.isRoundEnd)
            {
                var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
                if (list.Count() == 0)
                {
                    return;
                }
                if (Card == list.Last())
                {
                    await Card.Effect.Summon(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                }
            }
            return;
        }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex)
            {
                return;
            }
            vr = 0;
            await Task.CompletedTask;
            return;
        }
    }
}
