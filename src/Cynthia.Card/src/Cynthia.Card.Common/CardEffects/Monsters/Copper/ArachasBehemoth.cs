using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24010")]//蟹蜘蛛巨兽
    public class ArachasBehemoth : CardEffect, IHandlesEvent<AfterCardConsume>
    {//每当友军单位吞噬1个单位，便在随机排生成1只“蟹蜘蛛幼虫”。 一共可生效4次。
        public ArachasBehemoth(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardConsume @event)
        {
            //如果并非友军吞噬, 并且位于正确位置的话,不触发效果
            if ((@event.Source.PlayerIndex == PlayerIndex) && (Card.Status.CardRow.IsOnPlace()) && Card.Status.Countdown >= 1 && Game.GetRandomRow(PlayerIndex, out var rowIndex))
            {
                await Card.Effect.SetCountdown(offset: -1);
                //在随机排末尾生成
                await Game.CreateCard(CardId.ArachasHatchling, PlayerIndex, Game.GetRandomCanPlayLocation(PlayerIndex, true));
            }
            return;
        }
    }
}