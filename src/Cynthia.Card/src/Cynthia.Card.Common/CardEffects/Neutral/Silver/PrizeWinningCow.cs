using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13022")]//获奖奶牛
    public class PrizeWinningCow : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：在同排生成1个“羊角魔”。
        public PrizeWinningCow(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCard(CardId.Chort, PlayerIndex, @event.DeathLocation);
            return;
        }
    }
}