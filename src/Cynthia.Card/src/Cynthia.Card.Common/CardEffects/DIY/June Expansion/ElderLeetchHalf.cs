using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70086")]//exact copy of self-eater card to allow it not to proc himself when spawning the second half
    public class ElderLeetchHalf : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterTurnOver>
    {//
        public ElderLeetchHalf(GameCard card) : base(card) { }
        private bool used = false;
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {//Set countdown to 2
            await Card.Effect.SetCountdown(2);
            return;
        }
        //Split the self-eater every 2 turns
        public async Task HandleEvent(AfterTurnStart @event)
        {
            var unitdamage = Card.Status.Strength;
            var unitrest = 0;
            var row = Card.Status.CardRow;
            used = false;
            void Lesser(CardStatus status)
            {//define the strenght of the second selfeater that appears, rounding up the damage
                status.Strength = unitdamage + unitrest;
            }
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {//Split and Damage
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    unitrest = unitdamage / 2;
                    unitdamage = (unitdamage + 1) / 2;
                    unitrest = unitrest - unitdamage;
                    //A different token unit is spawned to avoid the card from self-proccing
                    await Card.Effect.SetCountdown(value: 2);
                    if (!(Game.RowToList(PlayerIndex, row).Count >= Game.RowMaxCount))
                    {
                        if (Card.Status.HealthStatus >= 0 && Card.Status.Strength > 1)
                        {
                            await Card.Effect.Weaken(unitdamage, Card);
                            await Game.CreateCard(CardId.ElderLeetchHalf, PlayerIndex, Card.GetLocation() + 1, setting: Lesser);
                        }
                        return;
                    }
                }
                return;
            }
            await Task.CompletedTask;
        }
        public async Task HandleEvent(AfterUnitDown @event)
        {//strengthen by 1 when you play a relict from hand
            if (@event.Target == Card) return;
            if (@event.Target.CardInfo().CardId == "70086") return;
            if (PlayerIndex == @event.Target.PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.Target.HasAllCategorie(Categorie.Relict) && @event.Target.CardInfo().CardId != "70086")
            {
                used = true;
                return;
            }
            await Task.CompletedTask;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == PlayerIndex && used)
            {
                await Strengthen(1, Card);
                await Game.ClientDelay(100);
            }
        }
    }
}