using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70041")]//鬼针草煎药
    public class BidensBipinnata : CardEffect
    {//Deal 3 damage to the highest enemy. Deal 2 additional damage to the highest unitfor each “白刺花” in your graveyard.

        public BidensBipinnata(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var count = Game.PlayersCemetery[PlayerIndex].Count(x => x.Status.CardId == CardId.AlbizziaJulibrissin);
            if (!Game.GetPlaceCards(AnotherPlayer).WhereAllHighest().TryMessOne(out var target1, Game.RNG))
                {
                    return 0;
                }
                await target1.Effect.Damage(3, Card);
            for (var i = 0; i < 3 + count; i++)
            {
                if (!Game.GetPlaceCards(AnotherPlayer).WhereAllHighest().TryMessOne(out var target, Game.RNG))
                {
                    break;
                }
                await target.Effect.Damage(2, Card);
            }

            return 0;
        }
    }
}
