using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130150")]//欧吉尔德·伊佛瑞克：晋升
    public class OlgierdVonEverecPro : CardEffect, IHandlesEvent<AfterCardToCemetery>, IHandlesEvent<BeforeRoundStart>, IHandlesEvent<AfterTurnOver>
    {//进入墓场时，复活至原位。
        public OlgierdVonEverecPro(GameCard card) : base(card) { }
        private CardLocation _toRecurretLocation = null;
        private bool _resurrectFlag = false;
        public async Task HandleEvent(AfterCardToCemetery @event)
        {
            // 进入墓地的不是本卡，什么都不发生
            if (@event.Target != Card)
            {
                return;
            }
            _toRecurretLocation = @event.DeathLocation;
            if (@event.isRoundEnd)
            {
                return;
            }
            _resurrectFlag = true;
            await Task.CompletedTask;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && _resurrectFlag == true)
            {
                //第一类情况，死亡进入墓地
                //复活到原位置
                if (_toRecurretLocation.RowPosition.IsOnPlace())
                {
                    await Card.Effect.Resurrect(_toRecurretLocation, Card);
                }
                //第二类情况，丢弃，随机复活到任何位置
                else
                {
                    await Card.Effect.Resurrect(Game.GetRandomCanPlayLocation(Card.PlayerIndex, true), Card);
                }
                _resurrectFlag = false;
            }
        }
        public async Task HandleEvent(BeforeRoundStart @event)
        {
            if (Card.Status.CardRow.IsInCemetery() && _toRecurretLocation != null)
            {
                await Card.Effect.Resurrect(_toRecurretLocation, Card);
            }
        }
    }
}