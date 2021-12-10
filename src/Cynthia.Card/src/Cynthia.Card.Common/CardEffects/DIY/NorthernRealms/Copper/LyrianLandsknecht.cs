using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70104")]//莱里亚长矛兵
    public class LyrianLandsknecht : CardEffect, IHandlesEvent<AfterRoundOver>
    {
        public LyrianLandsknecht(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterRoundOver @event)
        {
            if (Card.Status.HealthStatus <= 0 || Card.Status.IsLock || !Card.Status.CardRow.IsOnPlace()) return;
            int keepBoostNum = Math.Min(Card.Status.HealthStatus, 10);
            Card.Effect.Repair(true);
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count)), Card);
            await Card.Effect.Boost(keepBoostNum, Card);
        }

    }

}