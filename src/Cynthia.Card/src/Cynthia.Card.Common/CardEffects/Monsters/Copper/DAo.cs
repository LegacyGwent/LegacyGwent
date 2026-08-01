using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24020")]//地灵
    public class DAo : CardEffect, IHandlesEvent<AfterCardDeath>
    {//遗愿：在同排生成2个“次级地灵”。
        public DAo(GameCard card) : base(card) { }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card) return;
            var row = @event.DeathLocation.RowPosition;
            var list = Game.RowToList(PlayerIndex, row);
            //await Game.Debug($"这里是dao的效果,目前这一排有:{list.Count()}个单位,计划生成到位置:{list.Count}");
            await Game.CreateCard(CardId.LesserDAl, PlayerIndex, new CardLocation(row, list.Count));
            //await Game.Debug($"这里是dao的效果,目前这一排有:{list.Count()}个单位,计划生成到位置:{list.Count}");
            await Game.CreateCard(CardId.LesserDAl, PlayerIndex, new CardLocation(row, list.Count));
        }
    }
}