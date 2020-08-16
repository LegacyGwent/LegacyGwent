using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70009")]//蝠翼魔

    public class Fleder : CardEffect, IHandlesEvent<BeforeCardDamage>, IHandlesEvent<AfterCardHurt>
    {//从牌库召唤1张同名牌至同排。每当有铜色/银色敌方单位转为受伤状态时，获得1点增益。
        public Fleder(GameCard card) : base(card) { }

        private const int boostPoint = 1;

        private bool _targetIsHealth = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //如果牌库中有同名牌的话
            if (Game.PlayersDeck[PlayerIndex].Any(t => t.Status.CardId == Card.Status.CardId))
            {
                if (!Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).TryMessOne(out var card, Game.RNG))
                {
                    return 0;
                }
                await card.Effect.Summon(Card.GetLocation() + 1, Card);
                return 0;
            }
            return 0;
        }

        public async Task HandleEvent(BeforeCardDamage @event)
        {
            _targetIsHealth = false;
            if (@event.Target.PlayerIndex != PlayerIndex && Card.Status.CardRow.IsOnPlace() && @event.Target.IsAnyGroup(Group.Copper, Group.Silver) && !Card.IsDead)
            {
                if (@event.Target.Status.HealthStatus >= 0)
                {
                    _targetIsHealth = true;
                }
            }
            await Task.CompletedTask;
            return;
        }

        public async Task HandleEvent(AfterCardHurt @event)
        {
            if (_targetIsHealth && Card.Status.CardRow.IsOnPlace())
            {
                if (@event.Target.Status.HealthStatus < 0)
                {
                    await Card.Effect.Boost(boostPoint, Card);
                }
            }
            _targetIsHealth = false;
            return;
        }
    }
}
