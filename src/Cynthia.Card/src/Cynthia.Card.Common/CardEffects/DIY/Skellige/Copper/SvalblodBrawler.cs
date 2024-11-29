using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70092")]//斯瓦勃洛争斗者 SvalblodBrawler
    public class SvalblodBrawler : CardEffect
    {//xx
        public SvalblodBrawler(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
           var damages = 4;
           if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
           {
                damages = 6;
           }
            var row = Game.RowToList(AnotherPlayer, Card.Status.CardRow).IgnoreConcealAndDead();
            for (var i = 0; i < damages; i++)
            {
                var card = row.Where(x => x.IsAliveOnPlance()).Mess(Game.RNG).Take(1);
                if (card.Count() > 0)
                    await card.Single().Effect.Damage(1, Card);
            }
            return 0;
        }
    }
}
