using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44008")]//加强型投石机
    public class ReinforcedTrebuchet : CardEffect, IHandlesEvent<AfterTurnOver>
    {//At the end of your turn, damage a random enemy unit by 1. \nCrewed: Increase initial Damage by 1.
    //在你的回合结束时，对一个随机敌军单位造成1点伤害。\n已操控：初始伤害增加1点。
        public ReinforcedTrebuchet(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            var cards = Game.GetPlaceCards(AnotherPlayer);
            if (cards.Count() == 0)
            {
                return;
            }
            await cards.Mess(RNG).First().Effect.Damage(1 + Card.GetCrewedCount(), Card);
        }


    }
}