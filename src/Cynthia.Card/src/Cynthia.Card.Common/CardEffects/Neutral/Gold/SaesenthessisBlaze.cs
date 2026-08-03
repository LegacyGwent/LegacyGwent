using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12006")]//萨琪亚萨司：龙焰
    public class SaesenthessisBlaze : CardEffect
    {
        public SaesenthessisBlaze(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersHandCard[PlayerIndex].ToList();
            var count = cards.Count;
            foreach (var card in cards)
            {
                await card.Effect.Banish();
            }

            for (var drawn = 0; drawn < count; drawn++)
            {
                if (Game.PlayersDeck[PlayerIndex].Count == 0)
                {
                    await ReturnCemeteryUnitsToDeck();
                }

                if (Game.PlayersDeck[PlayerIndex].Count == 0)
                {
                    break;
                }

                await Game.PlayerDrawCard(PlayerIndex);
            }

            return 0;
        }

        private async Task ReturnCemeteryUnitsToDeck()
        {
            var units = Game.PlayersCemetery[PlayerIndex]
                .Where(card => card.Status.Type == CardType.Unit)
                .ToList();
            foreach (var unit in units)
            {
                if (!unit.Status.CardRow.IsInCemetery())
                {
                    continue;
                }

                var index = RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count + 1);
                await unit.Effect.Resurrect(
                    new CardLocation(RowPosition.MyDeck, index),
                    Card);
            }
        }
    }
}
