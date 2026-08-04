using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70191")]//达娜·梅碧 Dana Meadbh
    public class DanaMeadbh : CardEffect
    {
        public DanaMeadbh(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(deckCard => deckCard.CardInfo().Faction == Faction.Neutral)
                .ToList();
            if (!candidates.Any())
            {
                return 0;
            }

            var selected = await Game.GetSelectMenuCards(
                PlayerIndex,
                candidates,
                1,
                isCanOver: false);
            if (!selected.Any())
            {
                return 0;
            }

            await selected.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}
