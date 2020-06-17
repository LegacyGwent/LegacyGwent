using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70025")]//席安娜
    public class Syanna : CardEffect, IHandlesEvent<AfterUnitPlay>, IHandlesEvent<AfterUnitDown>
    {//4护甲。力竭。使你的下一张银色/铜色忠诚单位卡额外触发一次部署效果。

        private bool _isUse = false;

        public Syanna(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(4, Card);
            if (_isUse == false)
            {
                await Card.Effect.SetCountdown(1);
                _isUse = true;
            }
            return 0;
        }

        private GameCard _target = null;

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            //在场上,打出到己方场上,拥有倒计时
            if (!(Card.Status.CardRow.IsOnPlace() &&
                Card.Status.IsCountdown &&
                @event.PlayedCard.PlayerIndex == PlayerIndex &&
                @event.PlayedCard.IsAnyGroup(Group.Silver, Group.Copper) &&
                @event.PlayedCard.CardInfo().CardUseInfo == CardUseInfo.MyRow))
            {
                return;
            }
            await Card.Effect.SetCountdown(offset: -1);
            _target = @event.PlayedCard;
            await PlayStayCard(await _target.Effect.CardPlayEffect(@event.IsSpying, @event.IsReveal), false);
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && _target == @event.Target && _target != null))
            {
                return;
            }
            _target = null;
            await @event.Target.Effect.CardDownEffect(@event.IsSpying, false);
        }
    }
}
