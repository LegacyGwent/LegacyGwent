using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34023")]//迪尔兰士兵
    public class DaerlanSoldier : CardEffect, IHandlesEvent<AfterCardReveal>
    {//被己方揭示时直接打出至随机排，然后抽1张牌。
        public DaerlanSoldier(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (@event.Target != Card || @event.Source == null || Game.GameRound.ToPlayerIndex(Game) != PlayerIndex) return;
            var location = Game.GetRandomCanPlayLocation(Card.PlayerIndex, true);
            await Card.Effect.Summon(location, @event.Target);
            await Game.PlayerDrawCard(Card.PlayerIndex, 1);
        }
    }
}