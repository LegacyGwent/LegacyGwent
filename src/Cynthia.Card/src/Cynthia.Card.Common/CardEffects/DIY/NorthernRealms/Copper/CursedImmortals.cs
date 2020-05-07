using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70024")]//被诅咒的不朽者
    public class CursedImmortals : CardEffect, IHandlesEvent<AfterCardDeath>, IHandlesEvent<BeforeCardToCemetery>
    {//相邻诅咒单位被摧毁时，在同排最右侧生成一张“鬼灵”，然后削弱自身3点。

        public CursedImmortals(GameCard card) : base(card) { }

        private const int weakenPoint = 3;

        private CardLocation myLoc;

        public async Task HandleEvent(BeforeCardToCemetery @event)
        {
            // 因为在AfterCardDeath时，本卡的位置会移动，所以需要在之前的时点先存起来
            myLoc = Card.GetLocation();
            await Task.CompletedTask;
        }

        public async Task HandleEvent(AfterCardDeath @event)
        {

            if (Card.Status.CardRow.IsOnPlace())
            {
                var neighborCards = Card.GetRangeCard(1).ToList();
                CardLocation deathLoc = @event.DeathLocation;

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
