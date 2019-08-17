using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("54013")] //维里赫德旅先锋
    public class VriheddVanguard : CardEffect, IHandlesEvent<AfterCardSwap>
    {
        //使所有“精灵”友军获得1点增益。 每次被交换时，再次触发此能力。
        public VriheddVanguard(GameCard card) : base(card)
        {
        }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await BoostAllElf(boost);

            return 0;
        }

        private async Task BoostAllElf(int boost)
        {
            var targets = Game.GetPlaceCards(PlayerIndex)
                .Where(x => x != Card && x.CardInfo().Categories.Contains(Categorie.Elf)).ToList();

            foreach (var target in targets)
            {
                await target.Effect.Boost(1, Card);
            }
        }

        private const int boost = 1;

        public async Task HandleEvent(AfterCardSwap @event)
        {
            if (@event.HandCard != Card)
            {
                return;
            }

            await BoostAllElf(boost);
        }
    }
}