using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24017")]//狼人
    public class Werewolf : CardEffect, IHandlesEvent<AfterWeatherApply>
    {//接触“满月”后获得7点增益。 免疫。
        public Werewolf(GameCard card) : base(card) { }

        private bool isfullmoon = false;
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            Card.Status.IsImmue = true;
            await Card.Effect.SetCountdown(1);
            if (Game.GameRowEffect[Card.PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus == RowStatus.FullMoon)
            {
                isfullmoon = true;
                await Card.Effect.Boost(7, Card);
                await SetCountdown(offset: -1);
            }
            return;
        }

        public async Task HandleEvent(AfterWeatherApply @event)
        {
            //以下代码基于 向满月上打出满月不会buff
            //如果不在场上
            if (!Card.Status.IsCountdown)
            {
                return;
            }
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            //如果特效没有放置到狼人排 什么事情都不做
            if (!(@event.PlayerIndex == Card.PlayerIndex && @event.Row == Card.Status.CardRow))
            {
                return;
            }
            //如果特效不是满月,记录isfullmoon
            if (@event.Type != RowStatus.FullMoon)
            {

                isfullmoon = false;
                await Task.CompletedTask;
                return;
            }
            //如果特效是满月 且之前的特效不是满月
            if (@event.Type == RowStatus.FullMoon && !isfullmoon)
            {
                isfullmoon = true;
                await Card.Effect.Boost(7, Card);
                await SetCountdown(offset: -1);
                return;
            }
            return;
        }
    }
}