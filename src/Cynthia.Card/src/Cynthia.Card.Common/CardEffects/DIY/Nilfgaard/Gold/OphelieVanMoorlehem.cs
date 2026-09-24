using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId(CardId.OphelieVanMoorlehem)]
    public class OphelieVanMoorlehem : CardEffect
    {
        public OphelieVanMoorlehem(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(x => x.Is(type: CardType.Unit) && x.CardPoint() > 1)
                .ToList();
            if (!(await Game.GetSelectMenuCards(PlayerIndex, candidates)).TrySingle(out var deckUnit))
            {
                return 0;
            }

            var before = deckUnit.CardPoint();
            await deckUnit.Effect.Lower_Power_By(before - 1, Card);
            var lostPower = deckUnit.Status.CardRow.IsInDeck()
                ? Math.Max(0, before - Math.Max(0, deckUnit.CardPoint()))
                : Math.Max(0, before);
            if (lostPower <= 0)
            {
                return 0;
            }

            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selected.TrySingle(out var damageTarget))
            {
                return 0;
            }

            await damageTarget.Effect.Damage(lostPower, Card);
            if (!damageTarget.IsDead && damageTarget.Status.CardRow.IsOnPlace())
            {
                return 0;
            }

            var healTargets = Game.GetPlaceCards(PlayerIndex)
                .Concat(Game.PlayersHandCard[PlayerIndex])
                .Where(x => x.Is(type: CardType.Unit))
                .ToList();
            if ((await Game.GetSelectMenuCards(PlayerIndex, healTargets)).TrySingle(out var healTarget))
            {
                await healTarget.Effect.Heal(Card);
            }
            return 0;
        }
    }
}
