using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70042")]//合欢茎魔药
    public class AlbizziaJulibrissin : CardEffect
    {//Boost the lowest ally by 3. Then boost the lowest ally by 2, repeat 2 times. For each Gigascorpion decoction in your graveyard, repeat an additional time.

        public AlbizziaJulibrissin(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var count = Game.PlayersCemetery[PlayerIndex].Count(x => x.Status.CardId == CardId.BidensBipinnata);
            if (!Game.GetPlaceCards(AnotherPlayer).WhereAllLowest().TryMessOne(out var target1, Game.RNG))
                {
                    return 0;
                }
                await target1.Effect.Boost(3, Card);
            for (var i = 0; i < 3 + count; i++)
            {
                if (!Game.GetPlaceCards(PlayerIndex).WhereAllLowest().TryMessOne(out var target, Game.RNG))
                {
                    break;
                }
                await target.Effect.Boost(2, Card);
            }

            return 0;
        }
    }
}
