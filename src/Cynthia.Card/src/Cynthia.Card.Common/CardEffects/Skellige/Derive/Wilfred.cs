using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Text.RegularExpressions;

namespace Cynthia.Card
{
    [CardEffectId("65005")]//威尔弗雷德
    public class Wilfred : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：使1个友军随机单位获得3点强化。
        public Wilfred(GameCard card) : base(card) { }
        // public async Task HandleEvent(AfterCardDeath @event)
        // {
        //     if (@event.Target != Card)
        //     {
        //         return;
        //     }
        //     var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsMyRow() && x != Card).Mess(Game.RNG).ToList();

        //     if (cards.Count() == 0)
        //     {
        //         return;
        //         //await Game.Debug($"可强化卡数量{cards.Count()}");
        //     }
        //     var cardre = cards.First();
        //     await cardre.Effect.Strengthen(3, Card);

        //     // if (@event.Target != Card) 
        //     // {
        //     //     return;
        //     // }
        //     // var row = @event.DeathLocation.RowPosition;
        //     // var list = Game.RowToList(PlayerIndex, row);

        //     // await Game.CreateCard(CardId.LesserDAl, PlayerIndex, new CardLocation(row, list.Count));
        //     // await Game.CreateCard(CardId.LesserDAl, PlayerIndex, new CardLocation(row, list.Count));
        //     // return ;
        // }
        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            await Game.CreateCard(CardId.Chort, PlayerIndex, @event.DeathLocation);
            return;
        }
    }
}