using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70147")] //渴血鸟怪 Plumard
    public class Plumard : CardEffect,IHandlesEvent<AfterCardDrain>, IHandlesEvent<AfterCardHurt>, IHandlesEvent<AfterCardConsume> //, IHandlesEvent<BeforeCardDamage>
    {
        public Plumard(GameCard card) : base(card){}
        public async Task HandleEvent(AfterCardDrain @event)
        {
            if (@event.Target.Status.CardId != Card.Status.CardId && @event.Target.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.Target.Status.CardRow == Card.Status.CardRow)
            {
                await Card.Effect.Drain(1, @event.Source);
            }
            
            return;
        }
        private async Task SummonMyself() // task to be called to summon self
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
        public async Task HandleEvent(AfterCardHurt @event) // trigger the summon if a card is destroyed through drain or damage
        {
            if (Game.GameRound.ToPlayerIndex(Game) != PlayerIndex || !@event.Target.IsDead || @event.DamageType.IsHazard())
                {
                    return;
                }
            if (@event.Source.HasAnyCategorie(Categorie.Vampire)) // this check doesn't work if the previous ones aren't done before
                { 
                        await SummonMyself(); 
                }
            return;
        }
        public async Task HandleEvent (AfterCardConsume @event) // handle consume cases i.e. Detlaff Higher Vampire 2nd effect & Regis Higher
        {
            if (@event.Source.PlayerIndex == PlayerIndex && @event.Source.HasAnyCategorie(Categorie.Vampire))
                { 
                        await SummonMyself(); 
                }
            return;
        }
    }
}
