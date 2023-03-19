using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34010")]//军旗手
    public class StandardBearer : CardEffect, IHandlesEvent<AfterUnitPlay>, IHandlesEvent<AfterUnitDown>
    {//己方每打出1张“士兵”单位牌，便使1个友军单位获得2点增益。
        public StandardBearer(GameCard card) : base(card) { }

        private GameCard _tempCard;

        public async Task HandleEvent(AfterUnitPlay @event)
        {
            _tempCard = @event.PlayedCard;
            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterUnitDown @event)
        {
            if (!Card.Status.CardRow.IsOnPlace() || @event.Target == Card || (@event.Target != _tempCard)  || @event.IsSpying)
            {
                return;
            }
            if (@event.Target.PlayerIndex == Card.PlayerIndex &&
                @event.Target.Status.Categories.Contains(Categorie.Soldier))
            {
                var cards = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
                if (cards.Count == 0) return;
                await cards.Single().Effect.Boost(2, Card);
            }
        }
    }
}