using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70095")]
    public class LyrianArbalest : CardEffect
    {
        public LyrianArbalest(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selected = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
            if (!selected.TrySingle(out var target))
            {
                return 0;
            }

            if (target.CardPoint() < Card.CardPoint())
            {
                await target.Effect.Damage(Card.CardPoint() - target.CardPoint(), Card);
            }
            else if (target.Status.Armor > 0)
            {
                var removedArmor = target.Status.Armor;
                target.Status.Armor = 0;
                await Game.ShowCardIconEffect(target, CardIconEffectType.BreakArmor);
                await Game.SendEvent(new AfterCardSubArmor(target, removedArmor, Card));
                await Game.ShowSetCard(target);
                await Game.SendEvent(new AfterCardArmorBreak(target, Card));
            }

            return 0;
        }
    }
}
