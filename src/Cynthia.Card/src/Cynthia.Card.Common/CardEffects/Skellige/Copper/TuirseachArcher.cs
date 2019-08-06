using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64010")]//图尔赛克家族弓箭手
    public class TuirseachArcher : CardEffect
    {//对3个单位各造成1点伤害。
        public TuirseachArcher(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选取至多三个单位，如果不选，什么都不做
            var targets = await Game.GetSelectPlaceCards(Card, 3, selectMode: SelectModeType.AllRow);
            if (targets.Count() == 0)
            {
                return 0;
            }
            //同时造成伤害
            foreach (var target in targets)
            {
                await target.Effect.Damage(1, Card);
            }

            return 0;
        }
    }
}