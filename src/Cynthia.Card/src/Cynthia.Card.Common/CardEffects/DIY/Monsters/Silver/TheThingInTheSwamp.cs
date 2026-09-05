using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70088")]
    public class TheThingInTheSwamp : CardEffect, IHandlesEvent<AfterWeatherApply>
    {
        public TheThingInTheSwamp(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await ReturnFogletsAndApplyFog();
            return 0;
        }

        public async Task HandleEvent(AfterWeatherApply @event)
        {
            if (!Card.IsAliveOnPlance() ||
                @event.Type != RowStatus.TorrentialRain ||
                @event.PlayerIndex != AnotherPlayer ||
                Game.GameRound.ToPlayerIndex(Game) != PlayerIndex)
            {
                return;
            }

            await ReturnFogletsAndApplyFog();
        }

        private async Task ReturnFogletsAndApplyFog()
        {
            // Snapshot before moving: moving a card mutates the cemetery list.
            var foglets = Game.PlayersCemetery[PlayerIndex]
                .Where(card => card.Status.CardId == CardId.Foglet)
                .ToList();
            foreach (var card in foglets)
            {
                card.Effect.Repair();
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, Game.PlayersDeck[Card.PlayerIndex].Count), card);
            }
            var row = await Game.GetSelectRow(PlayerIndex, Card, TurnType.Enemy.GetRow());
            await Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()]
                .SetStatus<ImpenetrableFogStatus>();
        }
    }
}
