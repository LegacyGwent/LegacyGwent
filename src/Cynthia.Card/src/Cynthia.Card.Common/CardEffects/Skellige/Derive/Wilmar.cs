using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("65007")]//威尔玛
    public class Wilmar : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：若为对方回合，则在对面此排生成1头“熊”。
        public Wilmar(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            //如果死的不是本卡或flag为false
            if (@event.Target != Card || Game.GameRound.ToPlayerIndex(Game) == PlayerIndex)
            {
                return;
            }
            if (Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count >= Game.RowMaxCount)
            {
                return;
            }

            await Game.CreateCardAtEnd(CardId.Bear, AnotherPlayer, @event.DeathLocation.RowPosition);
            return;
        }
    }
}