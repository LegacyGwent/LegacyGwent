using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70106")]//安德莱格虫卵
    public class EndregaEggs : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardDeath>
    {//在左侧生成1张原始同名牌。遗愿：在同排生成1张“安德莱格幼虫”。3回合后，回合结束时，摧毁自身。
        public EndregaEggs(GameCard card) : base(card) { }

        private bool IsCopy { get; set; } = true;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            IsCopy = false;
            await Card.Effect.SetCountdown(value: 3);
            // we give it the doomed tag which is just display but not .IsDoomed = true, because we had to recode the ban to happen after the death, and if we set it to doomed, it will be banished instead of going to cemetery, and the death event won't trigger, so we have to check if it's copy or not in the death event handler, and only create larva if it's not copy
            for (var i = 0; i < 1; i++)
            {
                await Game.CreateCard(CardId.EndregaEggs, PlayerIndex, Card.GetLocation(), card => card.Categories = new Categorie[] { Categorie.Insectoid , Categorie.Doomed}); 
            }
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != PlayerIndex || !Card.IsAliveOnPlance())
            {
                return;
            }
            if (!Card.Status.IsCountdown)
            {
                return;
            }
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown <= 0)
            {
                await Card.Effect.ToCemetery(CardBreakEffectType.ToCemetery);
            }

        }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCard(CardId.EndregaLarva, PlayerIndex, @event.DeathLocation);
            if (IsCopy)
            {
                await Card.Effect.Banish();
            }
            return;
        }
    }
}