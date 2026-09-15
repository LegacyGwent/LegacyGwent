using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70153")]//莫拉汉姆家猎手 VanMoorleheHunter
    public class VanMoorleheHunter : CardEffect
    {//
        public VanMoorleheHunter(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.Is(Group.Copper, CardType.Unit) && x.CardPoint() > 1)
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
