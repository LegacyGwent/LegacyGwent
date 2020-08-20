using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;


namespace Cynthia.Card
{
    [CardEffectId("65004")]//幽灵鲸
    public class SpectralWhale : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardDeath>
    {//回合结束时移至随机排，对同排所有其他单位造成1点伤害。遗愿：再次触发此能力。间谍。
        public SpectralWhale(GameCard card) : base(card) { }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            //以下代码基于：如果满场，幽灵鲸不移动且不伤害。
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
            var damagelist = Game.RowToList(Card.PlayerIndex, resultrow).IgnoreConcealAndDead();
            await Card.Effect.Move(new CardLocation() { RowPosition = resultrow, CardIndex = int.MaxValue }, Card);
            foreach (var card in damagelist)
            {
                await card.Effect.Damage(1, Card);
            }
            return;
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {
            if (@event.Target != Card)
            {
                return;
            }
            var damagelist = Game.RowToList(Card.PlayerIndex, @event.DeathLocation.RowPosition).IgnoreConcealAndDead().Where(x => x.GetLocation().RowPosition.IsOnPlace() && x != Card).ToList();
            if (damagelist.Count() <= 1)
            {
                return;
            }
            foreach (var card in damagelist)
            {
                await card.Effect.Damage(1, Card);
            }
            return;
        }
    }
}