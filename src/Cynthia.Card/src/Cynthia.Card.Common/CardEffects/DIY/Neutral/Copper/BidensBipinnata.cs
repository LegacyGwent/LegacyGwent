using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70041")]//鬼针草煎药
    public class BidensBipinnata : CardEffect
    {//伤害最强的敌军单位2点，重复4次。 己方墓地每有一张“合欢茎魔药”，则额外重复1次。

        public BidensBipinnata(GameCard card) : base(card) { }

        public override async Task<int> CardUseEffect()
        {
            var count = Game.PlayersCemetery[PlayerIndex].Count(x => x.Status.CardId == CardId.AlbizziaJulibrissin);

            for (var i = 0; i < 4 + count; i++)
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
