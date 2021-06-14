using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("130100")]//奥克维斯塔：晋升
    public class OcvistPro : CardEffect, IHandlesEvent<AfterTurnStart>
    {//力竭。 2回合后的回合开始时：对所有敌军单位造成1点伤害，随后返回手牌。
        public OcvistPro(GameCard card) : base(card) { }

        private bool _isUse = false;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (_isUse)
            {
                return 0;
            }
            await Card.Effect.SetCountdown(Card.CardInfo().Countdown);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (!(@event.PlayerIndex == PlayerIndex && !_isUse && Card.Status.CardRow.IsOnPlace()))
            {
                return;
            }

            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown <= 0)
            {
                _isUse = true;
                Card.Status.IsCountdown = false;
                var cards = Game.GetAllCard(AnotherPlayer).Where(x => x.PlayerIndex == AnotherPlayer && x.Status.CardRow.IsOnPlace());
                foreach (var card in cards)
                {
                    await card.Effect.Damage(1, Card, BulletType.FireBall);
                }
                Card.Effect.Repair(true);
                await Game.ShowCardMove(new CardLocation(RowPosition.MyHand, 0), Card, refreshPoint: true);
            }
        }
    }
}