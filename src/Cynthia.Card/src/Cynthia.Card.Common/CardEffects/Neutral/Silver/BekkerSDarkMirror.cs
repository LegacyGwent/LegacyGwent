using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13026")]//贝克尔的黑暗之镜
    public class BekkerSDarkMirror : CardEffect, IHandlesEvent<AfterCardHurt>
    {//对场上最强的单位造成最多13点伤害（无视护甲），并使场上最弱的单位获得相同数值的增益。
        public BekkerSDarkMirror(GameCard card) : base(card) { }
        private GameCard boostCard;
        public override async Task<int> CardUseEffect()
        {
            var list = Game.GetAllPlaceCards().ToList();
            if (list.Count() <= 0) 
                return 0;
            var damageCard = list.WhereAllHighest().Mess(RNG).First();
            boostCard = list.WhereAllLowest().Mess(RNG).First();
            await damageCard.Effect.Damage(13, Card, BulletType.RedLight, true);//穿透伤害
            return 0;
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (@event.Source == Card && boostCard.Status.CardRow.IsOnPlace())
            {
                await boostCard.Effect.Boost(@event.Num, Card);
            }
            await Task.CompletedTask;
        } 
    }
}
