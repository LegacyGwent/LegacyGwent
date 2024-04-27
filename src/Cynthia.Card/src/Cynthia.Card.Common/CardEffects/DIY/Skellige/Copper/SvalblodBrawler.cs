using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70092")]//斯瓦勃洛争斗者 SvalblodBrawler
    public class SvalblodBrawler : CardEffect
    {//对1个敌军单位与自身造成4点伤害，若自身位于灾厄下则改为8点。
        public SvalblodBrawler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
           var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
           if (!selectList.TrySingle(out var target))
           {
                return 0;
           }

           var damages = 4;
           if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
           {
                damages = 8;
           }

           await target.Effect.Damage(damages, Card);

           return 0;
        }
    }
}
