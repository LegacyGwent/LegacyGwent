using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("24013")]//巨棘魔树
    public class Archespore : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardDeath>
    {//回合开始时移至随机排，对1个随机敌军单位造成1点伤害。 遗愿：对1个敌军随机单位造成4点伤害。
        public Archespore(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            //以下代码基于：如果满场，巨棘魔树不移动且不伤害。
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace())
            {
                return;
            }
            //所有排
            var allmylist = new List<RowPosition>() { RowPosition.MyRow1, RowPosition.MyRow2, RowPosition.MyRow3 };
            var plist = new List<RowPosition>();
            foreach (var row in allmylist)
            {
                if (Game.RowToList(Card.PlayerIndex, row).Count() < Game.RowMaxCount)
                {
                    plist.Add(row);
                }
            }
            if (plist.Count() == 0)
            {
                return;
            }
            //现在位置
            var nowpositon = new List<RowPosition>() { Card.Status.CardRow };
            //做差集得到可移动位置
            var movelist = plist.Except(nowpositon);
            //如果没有可移动位置，结束
            if (movelist.Count() == 0)
            {
                return;
            }
            var resultrow = movelist.Mess(Game.RNG).First();
            await Card.Effect.Move(new CardLocation() { RowPosition = resultrow, CardIndex = int.MaxValue }, Card);

            //伤害敌方场上随机卡
            var enemycards = Game.GetPlaceCards(AnotherPlayer).ToList();
            if (enemycards.Count() == 0)
            {
                return;
            }
            await enemycards.Mess(Game.RNG).First().Effect.Damage(1, Card);
            return;
        }


        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            //伤害敌方场上随机卡
            var enemycards = Game.GetPlaceCards(AnotherPlayer).ToList();
            if (enemycards.Count() == 0)
            {
                return;
            }
            await enemycards.Mess(Game.RNG).First().Effect.Damage(4, Card);
            return;
        }
    }
}