using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12017")]//杰洛特：猎魔大师
    public class GeraltProfessional : CardEffect
    {//Deploy: toggle a unit's lock, then damage it by 4. If the unit is a Monster, destroy it instead.
        public GeraltProfessional(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow, isHasConceal: true );
            if (cards.Count == 0) return 0;
            var card = cards.Single();
            await card.Effect.Lock(Card);
            //If the unit is a Monster, destroy it instead.
            if (card.Status.Faction == Faction.Monsters)
            {
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            //then damage it by 4.
            else
            {
                await card.Effect.Damage(4, Card);
            }
            return 0;
        }
    }
}