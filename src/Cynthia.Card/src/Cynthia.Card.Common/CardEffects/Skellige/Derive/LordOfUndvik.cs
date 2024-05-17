using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("65003")]//乌德维克之主
    public class LordOfUndvik : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：使“哈尔玛”获得10点增益。
        public LordOfUndvik(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            //以下代码基于：如果对方场上有很多哈尔马全都强化
            if (@event.Target != Card || !@event.DeathLocation.RowPosition.IsOnPlace())
            {
                return;
            }

            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex != Card.PlayerIndex && x.CardInfo().CardId == "62002").Mess(Game.RNG).ToList();
            foreach (var card in cards)
            {
                if (card.Status.CardRow.IsOnPlace())
                {
                    await card.Effect.Boost(14, Card);
                    card.Status.IsImmue = true;
                }
            }
            return;


        }


    }
}