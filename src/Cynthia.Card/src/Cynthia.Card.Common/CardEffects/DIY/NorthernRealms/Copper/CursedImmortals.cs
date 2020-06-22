using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70024")]//被诅咒的不朽者
    public class CursedImmortals : CardEffect, IHandlesEvent<BeforeCardToCemetery>
    {//相邻诅咒单位被摧毁时，在同排最右侧生成一张“鬼灵”
        public CursedImmortals(GameCard card) : base(card) { }
        private const int weakenPoint = 3;

        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            // 因为在AfterCardDeath时，本卡的位置会移动，所以选择在BeforeCardToCemetery的时点触发
            if (Card.Status.CardRow.IsOnPlace() && !@event.isRoundEnd && @event.Target != Card && @event.Target.PlayerIndex == PlayerIndex)
            {
                CardLocation myLoc = Card.GetLocation();
                CardLocation deathLoc = @event.DeathLocation;
                var neighborCards = Card.GetRangeCard(1).ToList();

                // if分开来写可读性更高
                // 如果相邻
                if (deathLoc.RowPosition == myLoc.RowPosition && Math.Abs(deathLoc.CardIndex - myLoc.CardIndex) <= 1)
                {
                    // 如果是诅咒
                    if (@event.Target.HasAnyCategorie(Categorie.Cursed))
                    {
                        // 如果没有满
                        if (Game.RowToList(PlayerIndex, deathLoc.RowPosition).Count < Game.RowMaxCount)
                        {
                            await Game.CreateCardAtEnd(CardId.Specter, PlayerIndex, deathLoc.RowPosition);
                            await Card.Effect.Weaken(weakenPoint, Card);
                        }
                    }
                }
            }
            return;
        }
    }
}