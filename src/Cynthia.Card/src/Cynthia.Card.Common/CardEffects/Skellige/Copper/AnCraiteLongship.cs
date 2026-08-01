using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64018")]//奎特家族作战长船
    public class AnCraiteLongship : CardEffect, IHandlesEvent<AfterCardDiscard>
    {//对1个敌军随机单位造成2点伤害。己方每丢弃1张牌，便触发此能力一次。
        public AnCraiteLongship(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await DamageRandomEnemy();
            return 0;
        }

        public async Task HandleEvent(AfterCardDiscard @event)
        {
            if (@event.Source.PlayerIndex == Card.PlayerIndex && Card.Status.CardRow.IsOnPlace())
            {
                await DamageRandomEnemy();
            }
            return;

        }

        private async Task DamageRandomEnemy()
        {
            var cards = Game.GetPlaceCards(AnotherPlayer);
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(2, Card);
        }
    }
}