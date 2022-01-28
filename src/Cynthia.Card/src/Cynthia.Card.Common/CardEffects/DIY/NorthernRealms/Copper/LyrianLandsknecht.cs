using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70104")]//莱里亚长矛兵
    public class LyrianLandsknecht : CardEffect, IHandlesEvent<AfterRoundOver>
    {//小局结束时，如果具有增益，则洗回牌组并保留至多10点增益
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