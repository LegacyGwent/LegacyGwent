using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70151")]//菲利普·凡·莫拉汉姆 PhilippevanMoorlehem
    public class PhilippevanMoorlehem : CardEffect
    {//
        public PhilippevanMoorlehem(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.Is(type: CardType.Unit) && x.CardPoint() > 1)
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, candidates)).TrySingle(out var deckUnit))
            {
                return 0;
            }

            var before = deckUnit.CardPoint();
            var requestedDamage = (before + 1) / 2;
            await deckUnit.Effect.Damage(requestedDamage, Card);
            var lostPower = deckUnit.Status.CardRow.IsInDeck()
                ? Math.Max(0, before - Math.Max(0, deckUnit.CardPoint()))
                : Math.Max(0, before);
            if (lostPower <= 0)
            {
                return 0;
            }

            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (selectList.TrySingle(out var target))
            {
                await target.Effect.Damage(lostPower, Card);
            }
            return 0;
        }
    }
}
