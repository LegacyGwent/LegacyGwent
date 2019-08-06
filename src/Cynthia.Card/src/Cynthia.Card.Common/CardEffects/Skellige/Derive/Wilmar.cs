using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("65007")]//威尔玛
    public class Wilmar : CardEffect, IHandlesEvent<AfterCardDeath>
    //, IHandlesEvent<AfterTurnStart>
    {//遗愿：若为对方回合，则在对面此排生成1头“熊”。 间谍。
        public Wilmar(GameCard card) : base(card) { }

        //一个记录现在是谁的回合的flag
        // private bool _summonflag = true;
        // public async Task HandleEvent(AfterTurnStart @event)
        // {
        //     //在回合交替时改变flag
        //     if (@event.PlayerIndex == Card.PlayerIndex)
        //     {
        //         _summonflag = false;
        //     }
        //     else
        //     {
        //         _summonflag = true;
        //     }
        //     await Task.CompletedTask;
        // }
        // public async Task HandleEvent(AfterCardDeath @event)
        // {
        //     //如果死的不是本卡或flag为false
        //     if (@event.Target != Card || !_summonflag)
        //     {
        //         return;
        //     }
        //     if (Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count >= 9)
        //     {
        //         return;
        //     }

        //     await Game.CreateCardAtEnd("15010", PlayerIndex, Card.Status.CardRow);
        //     return;

        // }


        //尝试使用羊角魔代码，失效
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCard(CardId.Chort, PlayerIndex, @event.DeathLocation);
            return;
        }
    }
}