using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("43009")]//欧德林
    public class Odrin : CardEffect, IHandlesEvent<AfterTurnStart>, IHandlesEvent<AfterCardDeath>
    {//回合开始时，移至随机排，并使同排所有友军单位获得1点增益。遗愿：使同排所有友军单位获得1点增益。
        public Odrin(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnStart @event)
        {
            //以下代码基于：如果满场，不移动且不增益，移动默认位置为最右侧
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
            if (movelist.Count() == 0)
            {
                return;
            }
            var resultrow = movelist.Mess(Game.RNG).First();
            var boostlist = Game.RowToList(Card.PlayerIndex, resultrow).IgnoreConcealAndDead();
            await Card.Effect.Move(new CardLocation() { RowPosition = resultrow, CardIndex = int.MaxValue }, Card);
            foreach (var card in boostlist)
            {
                await card.Effect.Boost(1, Card);
            }
            return;
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            var boostlist = Game.RowToList(Card.PlayerIndex, @event.DeathLocation.RowPosition).IgnoreConcealAndDead().Where(x => x.GetLocation().RowPosition.IsOnPlace() && x != Card).ToList();
            if (boostlist.Count() <= 1)
            {
                return;
            }
            foreach (var card in boostlist)
            {
                await card.Effect.Boost(1, Card);
            }
            return;
        }

    }
}