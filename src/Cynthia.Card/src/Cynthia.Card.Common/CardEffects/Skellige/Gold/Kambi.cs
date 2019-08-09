using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("62013")]//坎比
    public class Kambi : CardEffect
    {//间谍。遗愿：生成“汉姆多尔”。
        public Kambi(GameCard card) : base(card) { }
        // public async Task HandleEvent(AfterCardDeath @event)
        // {
        //     //如果死的不是这样卡，或者死亡位置不是场上，什么事情都不做
        //     if (@event.Target != Card || !@event.DeathLocation.RowPosition.IsOnPlace())
        //     {
        //         return;
        //     }

        //     var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace());
        //     if (cards.Count() != 0)
        //     {
        //         foreach (var card in cards)
        //         {
        //             await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
        //         }
        //     }
        //     var allrow = new List<int>() { 0, 1, 2, 3, 4, 5 };
        //     foreach (var rowIndex in allrow)
        //     {
        //         //将所有行设置为无效果
        //         if (Game.GameRowEffect[PlayerIndex][rowIndex].RowStatus != RowStatus.None)
        //         {
        //             await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<NoneStatus>();
        //         }
        //     }

        //     var row = @event.DeathLocation.RowPosition;
        //     //最左生成汉姆多尔
        //     await Game.CreateCard(CardId.Hemdall, Card.PlayerIndex, new CardLocation(row, 0));
        // }
    }
}