using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44016")]//亚甸槌击者
    public class AedirnianMauler : CardEffect
    {//对1个敌军造成4点伤害。
        public AedirnianMauler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);

            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(4, Card);
            return 0;
        }
    }
}