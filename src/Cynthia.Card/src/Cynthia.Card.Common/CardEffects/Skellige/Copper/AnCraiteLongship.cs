using System;
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
            // 现在触发条件：触发源在己方半场且不是密探，或者触发源在对方半场且是密探
            // 出新卡时注意可能涉及bug
            if (Card.Status.CardRow.IsOnPlace()
            && (@event.Source.PlayerIndex == Card.PlayerIndex && !@event.Source.HasAnyCategorie(Categorie.Agent)
            || (@event.Source.PlayerIndex != Card.PlayerIndex && @event.Source.HasAnyCategorie(Categorie.Agent))))
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