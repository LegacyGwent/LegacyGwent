using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34016")]//射石机
    public class Mangonel : CardEffect, IHandlesEvent<AfterCardReveal>
    {//对1个敌军随机单位造成2点伤害。己方每揭示1张牌，便再次触发此能力。
        public Mangonel(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            await DamageRandomEnemy();
            return 0;
        }

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (@event.Source == null || @event.Source.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;
            await DamageRandomEnemy();
        }

        private async Task DamageRandomEnemy()
        {
            var cards = Game.GetAllCard(Card.PlayerIndex)
                .Where(x => x.PlayerIndex != Card.PlayerIndex && x.Status.CardRow.IsOnPlace());
            if (cards.Count() == 0) return;
            await cards.Mess().First().Effect.Damage(2, Card);
        }
    }
}