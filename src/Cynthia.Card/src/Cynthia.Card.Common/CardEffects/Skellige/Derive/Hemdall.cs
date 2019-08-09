using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("65001")]//汉姆多尔


    //妥协的写法，回合结束生效
    public class Hemdall : CardEffect, IHandlesEvent<AfterTurnOver>
    {//摧毁场上所有单位，并移除所有恩泽和灾厄。
        public Hemdall(GameCard card) : base(card) { }

        private bool _isuse = false;
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            _isuse = false;
            await Task.CompletedTask;

        }



        public async Task HandleEvent(AfterTurnOver @event)
        {

            if (_isuse == true)
            {
                return;
            }
            _isuse = true;
            //列出场上其他卡
            var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x != Card).ToList();
            if (cards.Count() != 0)
            {
                foreach (var card in cards)
                {
                    await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                }
            }
            var allrow = new List<int>() { 0, 1, 2, 3, 4, 5 };
            foreach (var rowIndex in allrow)
            {
                //将所有行设置为无效果
                await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<NoneStatus>();
            }
        }
    }
}



// var cards = Game.GetAllCard(Card.PlayerIndex).Where(x => x.Status.CardRow.IsOnPlace() && x != Card).ToList();
// if (cards.Count() == 0)
// {
//     return;
// }
// foreach (var card in cards)
// {
//     await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
// }



// var allrow = new List<int>() { 0, 1, 2, 3, 4, 5 };
// foreach (var rowIndex in allrow)
// {
//     //将所有行设置为无效果
//     await Game.GameRowEffect[PlayerIndex][rowIndex].SetStatus<NoneStatus>();
// }
// return;