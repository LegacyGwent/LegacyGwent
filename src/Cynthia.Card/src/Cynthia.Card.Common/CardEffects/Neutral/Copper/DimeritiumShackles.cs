using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("14007")]//阻魔金镣铐
    public class DimeritiumShackles : CardEffect
    {//改变1个单位的锁定状态。若为敌军单位，则对它造成5点伤害。
        public DimeritiumShackles(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectPlaceCards(Card, isHasConceal: true);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Lock(Card);
            if (result.Single().PlayerIndex != Card.PlayerIndex)
                await result.Single().Effect.Damage(5, Card);
            return 0;
        }
    }
}