using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("41001")]//拉多维德五世
    public class KingRadovidV : CardEffect
    {//改变2个单位的锁定状态。若为敌军单位，则对其造成5点伤害。 操控。
        public KingRadovidV(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //选取至多两个单位，如果不选，什么都不做
            var targets = await Game.GetSelectPlaceCards(Card, 2, selectMode: SelectModeType.AllRow, isHasConceal: true);
            if (targets.Count() == 0)
            {
                return 0;
            }
            //同时造成伤害和锁定
            foreach (var target in targets)
            {
                await target.Effect.Lock(Card);
                if (target.PlayerIndex != Card.PlayerIndex)
                {
                    await target.Effect.Damage(5, Card);
                }
            }

            return 0;
        }
    }
}