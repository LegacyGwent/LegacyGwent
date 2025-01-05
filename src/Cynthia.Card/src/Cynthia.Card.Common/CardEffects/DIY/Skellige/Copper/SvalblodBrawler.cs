using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70092")]//斯瓦勃洛争斗者 SvalblodBrawler
    public class SvalblodBrawler : CardEffect
    {//Deploy: Damage an enemy by twice the number of Torrential Rains on the board.
        public SvalblodBrawler(GameCard card) : base(card) { }
        private const int increment = 2;
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var count = Game.GameRowEffect.SelectMany(x => x.Select(x => x.RowStatus)).Where(x => x == RowStatus.TorrentialRain).Count();
            if (count>0)
            {
                var result = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
                if (result.Count != 0) await result.Single().Effect.Damage(increment * count, Card);
            }
            return 0;
        }
    }
}
