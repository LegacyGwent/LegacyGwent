using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13026")]//贝克尔的黑暗之镜
    public class BekkerSDarkMirror : CardEffect, IHandlesEvent<AfterCardHurt>
    {//对场上最强的单位造成最多10点伤害（无视护甲），并使场上最弱的单位获得相同数值的增益。
        public BekkerSDarkMirror(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.GetAllCard(PlayerIndex).ToList();
            if (list.Count() <= 0) return 0;
            //穿透伤害
            var damageCard = list.WhereAllHighest().Mess(RNG).First();
            await damageCard.Effect.Damage(10, Card, BulletType.RedLight, true);
            return 0;
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (@event.Source == Card)
            {
                var list = Game.GetAllCard(PlayerIndex).ToList();
                var boostCard = list.WhereAllLowest().Mess(RNG).First();
                await boostCard.Effect.Boost(@event.Num, Card);
            }
        }
    }
}