using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44008")]//加强型投石机
    public class ReinforcedTrebuchet : CardEffect, IHandlesEvent<AfterTurnOver>
    {//回合结束时，对1个敌军随机单位造成1点伤害。
        public ReinforcedTrebuchet(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            if (!Card.Status.CardRow.IsOnPlace())
            {
                return 0;
            }

            for (var i = 0; i < Card.GetCrewedCount(); i++)
            {
                await Card.Effect.Strengthen(1, Card);
            }

            return 0;
        }

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
            await cards.Mess(RNG).First().Effect.Damage(1, Card);
        }


    }
}
