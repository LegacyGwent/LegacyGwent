using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70025")]//席安娜
    public class Syanna : CardEffect, IHandlesEvent<AfterUnitPlay>, IHandlesEvent<AfterUnitDown>, IHandlesEvent<AfterRoundPlay>
    {//4护甲。力竭。使你的下一张银色/铜色忠诚单位牌额外触发一次部署效果。

        public Syanna(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(4, Card);
            await Card.Effect.SetCountdown(1);
            return 0;
        }

        private GameCard _target = null;
        private AfterUnitPlay _after_unit_play = null;
        private AfterUnitDown _after_unit_down = null;


        public async Task HandleEvent(AfterUnitPlay @event)
        {
            //在场上,打出到己方场上,拥有倒计时
            if (!(Card.Status.CardRow.IsOnPlace() &&
                Card.Status.IsCountdown &&
                @event.PlayedCard.PlayerIndex == PlayerIndex &&
                @event.PlayedCard.IsAnyGroup(Group.Silver, Group.Copper) &&
                @event.PlayedCard.CardInfo().CardUseInfo == CardUseInfo.MyRow && _target == null))
            {
                return;
            }

            _target = @event.PlayedCard;
            if (_after_unit_play == null)
                _after_unit_play = @event;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && _target == @event.Target && _target != null))
            {
                return;
            }

            _target = null;

            if (_after_unit_down == null)
                _after_unit_down = @event;
        }

        public async Task HandleEvent(AfterRoundPlay @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            if(_after_unit_down != null || _after_unit_play != null)
                await Card.Effect.SetCountdown(offset: -1);
            if (_after_unit_play != null)
            {
                await PlayStayCard(await _after_unit_play.PlayedCard.Effect.CardPlayEffect(_after_unit_play.IsSpying, _after_unit_play.IsReveal), false);
                _after_unit_play = null;
            }
            if (_after_unit_down != null)
            {
                await _after_unit_down.Target.Effect.CardDownEffect(_after_unit_down.IsSpying, false);
                _after_unit_down = null;
            }
        }
    }
}
