using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70010")]//原蝠翼魔

    public class Protofleder : CardEffect, IHandlesEvent<BeforeCardDamage>, IHandlesEvent<AfterCardHurt>
    {//部署：生成1张“蝠翼魔”并将其置于你的牌库顶。每当有敌方单位转为受伤状态时，获得2点增益。
        public Protofleder(GameCard card) : base(card) { }

        private const int boostPoint = 2;

        private bool _targetIsHealth = false;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(CardId.Fleder, Card.PlayerIndex, new CardLocation(RowPosition.MyDeck, 0));
            return 0;
        }

        public async Task HandleEvent(BeforeCardDamage @event)
        {
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
