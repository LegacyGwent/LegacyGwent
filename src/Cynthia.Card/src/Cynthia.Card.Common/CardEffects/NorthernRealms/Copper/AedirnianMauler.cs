using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44016")]//亚甸槌击者
    public class AedirnianMauler : CardEffect
    {//damage an enemy by 4, if it survives damage another by 2
        public AedirnianMauler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);

            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(4, Card);
            if (target.CardPoint() > 0)
            {
            var target2 = (await Game.GetSelectPlaceCards(Card, filter: (x => x != target), selectMode: SelectModeType.EnemyRow)).SingleOrDefault();
            if (target2 == default)
            {
                return 0;
            }
            await target2.Effect.Damage(2, Card);
            }
            return 0;
        }
    }
}