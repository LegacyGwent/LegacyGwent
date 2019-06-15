using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54009")] //多尔·布雷坦纳射手
    public class DolBlathannaBowman : CardEffect
    {
        //对1个敌军单位造成2点伤害。 每当有敌军单位改变所在排别，便对其造成2点伤害。 自身移动时对1个敌军随机单位造成2点伤害。
        public DolBlathannaBowman(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        private const int damage = 2;
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (list.Count <= 0) return 0;
            await list.First().Effect.Damage(damage);
            return 0;
        }

        public override async Task OnCardMove(GameCard target, GameCard source = null)
        {
            if (target.PlayerIndex != Card.PlayerIndex)
            {
                await target.Effect.Damage(damage);
            }
        }
    }
}