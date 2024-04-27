using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70128")]//月之尘炸弹 MoonDust 
    public class MoonDust : CardEffect
    {//
        public MoonDust(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Damage(5,Card);
            for (var i = 0; i < 4; i++)
            {
                if (!Game.GetPlaceCards(AnotherPlayer).TryMessOne(out var target, Game.RNG))
                {
                    break;
                }
                await target.Effect.Damage(1, Card);
            }
			return 0;
        }
    }
}
