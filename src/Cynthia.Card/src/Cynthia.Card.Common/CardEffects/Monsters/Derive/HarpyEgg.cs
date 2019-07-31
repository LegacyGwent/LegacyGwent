using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("25004")]//鹰身女妖蛋
    public class HarpyEgg : CardEffect, IHandlesEvent<AfterCardDeath>, IHandlesEvent<AfterCardConsume>
    {//使吞噬自身的单位获得额外4点增益。 遗愿：在随机排生成1只“鹰身女妖幼崽”。
        public HarpyEgg(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            if (!Game.GetRandomRow(PlayerIndex, out var rowIndex))
            {
                return;
            }
            await Game.CreateCardAtEnd(CardId.HarpyHatchling, PlayerIndex, rowIndex.Value);
        }

        public async Task HandleEvent(AfterCardConsume @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            await @event.Source.Effect.Boost(4, Card);
        }
    }
}