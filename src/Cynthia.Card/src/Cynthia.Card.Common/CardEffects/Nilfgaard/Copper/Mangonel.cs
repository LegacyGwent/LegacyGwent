using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34016")]//射石机
    public class Mangonel : CardEffect, IHandlesEvent<AfterCardReveal>
    {//对1个敌军随机单位造成2点伤害。己方每揭示1张牌，便再次触发此能力。
        public Mangonel(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await DamageRandomEnemy();
            return 0;
        }

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (@event.Source == null || (@event.Source.PlayerIndex != Card.PlayerIndex && @event.Source.Status.CardId != "70174") || !Card.Status.CardRow.IsOnPlace()) return;
            await DamageRandomEnemy();
        }

        private async Task DamageRandomEnemy()
        {
            var cards = Game.GetPlaceCards(AnotherPlayer);
            if (cards.Count() == 0) return;
            await cards.Mess(Game.RNG).First().Effect.Damage(2, Card);
        }
    }
}