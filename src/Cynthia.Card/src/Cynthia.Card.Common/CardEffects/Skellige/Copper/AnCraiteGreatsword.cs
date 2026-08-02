using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    public abstract class AnCraiteGreatswordEffect : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        protected AnCraiteGreatswordEffect(GameCard card) : base(card) { }

        protected abstract int TurnCountdown { get; }
        protected abstract int StrengthenAmount { get; }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: TurnCountdown);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await Card.Effect.SetCountdown(offset: -1);
                if (Card.Effect.Countdown <= 0)
                {
                    //重新倒计时
                    await Card.Effect.SetCountdown(value: TurnCountdown);
                    //如果受伤，触发效果
                    if (Card.Status.HealthStatus < 0)
                    {
                        await Card.Effect.Heal(Card);
                        await Card.Effect.Strengthen(StrengthenAmount, Card);
                    }
                }
            }
        }
    }

    [CardEffectId("64009")]//奎特家族巨剑士（Z／原版）
    public class AnCraiteGreatsword : AnCraiteGreatswordEffect
    {
        public AnCraiteGreatsword(GameCard card) : base(card) { }
        protected override int TurnCountdown => 2;
        protected override int StrengthenAmount => 2;
    }

    [CardEffectId("64035")]//奎特家族巨剑士A
    public class AnCraiteGreatswordA : AnCraiteGreatswordEffect
    {
        public AnCraiteGreatswordA(GameCard card) : base(card) { }
        protected override int TurnCountdown => 3;
        protected override int StrengthenAmount => 2;
    }

    [CardEffectId("64036")]//奎特家族巨剑士B
    public class AnCraiteGreatswordB : AnCraiteGreatswordEffect
    {
        public AnCraiteGreatswordB(GameCard card) : base(card) { }
        protected override int TurnCountdown => 3;
        protected override int StrengthenAmount => 3;
    }

    [CardEffectId("64037")]//奎特家族巨剑士C
    public class AnCraiteGreatswordC : AnCraiteGreatswordEffect
    {
        public AnCraiteGreatswordC(GameCard card) : base(card) { }
        protected override int TurnCountdown => 2;
        protected override int StrengthenAmount => 2;
    }
}
