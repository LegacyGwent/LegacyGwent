using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70185")]//地灵
    public class NoonWraith : CardEffect, IHandlesEvent<AfterCardDeath>
    {//Deathwish: Summon 3 Rats to the opponent' row.
        public NoonWraith(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCardAtEnd(CardId.Rat, AnotherPlayer, @event.DeathLocation.RowPosition);
            await Game.CreateCardAtEnd(CardId.Rat, AnotherPlayer, @event.DeathLocation.RowPosition);
            await Game.CreateCardAtEnd(CardId.Rat, AnotherPlayer, @event.DeathLocation.RowPosition);
        }
    }
}