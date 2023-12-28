using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70128")]//月之尘炸弹 ‎MoonDust
    public class ‎MoonDust : CardEffect,IHandlesEvent<BeforeSpecialPlay>, IHandlesEvent<BeforePlayStayCard>
    {//摧毁1个战力不高于5的敌军单位，己方打出谋略牌时，复活并放逐1张同名牌

        public BasiliskVenom(GameCard card) : base(card) { }
        private GameCard _discardSource = null;
        private int _resurrectCount = 0;
        private int _PlayCount = 0;

        public override async Task<int> CardUseEffect()
        {
            var result = (await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.EnemyRow, filter: x => x.CardPoint() < 6));
            if(result.Count() != 0)
            {
                await result.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            
            if (_PlayCount == 1)
            {
                Card.Status.IsDoomed = true;
            }
            _PlayCount++;
            
            return 0;
        }

        public async Task HandleEvent(BeforeSpecialPlay @event)
        {
            if (@event.Target.PlayerIndex == Card.PlayerIndex && @event.Target.Status.Group == Group.Copper && @event.Target.CardInfo().Categories.Contains(Categorie.Tactic) && Card.Status.CardRow.IsInCemetery())
            {
                await Card.Effect.Resurrect(CardLocation.MyStayFirst, Card);
                _resurrectCount++;
                _discardSource = Card;
                //await Game.CreateCard(CardId.BasiliskVenom, PlayerIndex,  new CardLocation(RowPosition.MyStay, 0), x => x.IsDoomed = true);
            }
            return;
        }

        public async Task HandleEvent(BeforePlayStayCard @event)
        {
            if (_discardSource == Card && _discardSource != null)
            {
                @event.PlayCount += _resurrectCount;
                _discardSource = null;
                _resurrectCount = 0;
            }
            await Task.CompletedTask;
        }
    }
}
