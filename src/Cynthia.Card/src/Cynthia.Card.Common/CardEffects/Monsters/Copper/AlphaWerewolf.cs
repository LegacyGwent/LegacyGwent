using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
namespace Cynthia.Card
{
    [CardEffectId("24007")]//狼人头领
    public class AlphaWerewolf : CardEffect
    {//接触“满月”效果时，在自身两侧各生成1只“狼”。
        public AlphaWerewolf(GameCard card) : base(card) { }
        private bool isfullmoon = false;
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            
            if (Game.GameRowEffect[Card.PlayerIndex][Card.GetLocation().RowPosition.MyRowToIndex()].RowStatus == RowStatus.FullMoon)
            {
                isfullmoon = true;
                await Game.CreateCard(CardId.Wolf, PlayerIndex, Card.GetLocation());
                await Game.CreateCard(CardId.Wolf, PlayerIndex, Card.GetLocation() + 1);
            }
            return;
        }

        public async Task HandleEvent(AfterWeatherApply @event)
        {
            //以下代码基于 向满月上打出满月不会再次生成狼

            //如果本卡不在场上
            if (!Card.GetLocation().RowPosition.IsOnPlace())
            {
                return;
            }
            //如果特效没有放置到狼人排 什么事情都不做
            if (!(@event.PlayerIndex == Card.PlayerIndex && @event.Row == Card.GetLocation().RowPosition))
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
                await Game.CreateCard(CardId.Wolf, PlayerIndex, Card.GetLocation());
                await Game.CreateCard(CardId.Wolf, PlayerIndex, Card.GetLocation() + 1);
                return;
            }
            return;
        }
    }
}