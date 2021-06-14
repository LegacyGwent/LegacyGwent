using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130220")]//获奖奶牛
    public class PrizeWinningCowPro : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：在同排生成1个“羊角魔”，并对对方同排所有单位造成2点伤害。
        public PrizeWinningCowPro(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCard(CardId.Chort, PlayerIndex, @event.DeathLocation);
            //对方同排列表
            var row = Game.RowToList(PlayerIndex, @event.DeathLocation.RowPosition.Mirror()).IgnoreConcealAndDead();
            foreach (var it in row)
            {
                await it.Effect.Damage(2, Card);
            }
            return;
        }
    }
}