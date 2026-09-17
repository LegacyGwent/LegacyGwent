using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70082")]//背亲者恩约夫 ArnjolfthePatricide
    public class ArnjolfthePatricide : CardEffect
    {
        public ArnjolfthePatricide(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var targets = Game.GetAllPlaceCards()
                .Where(x => x.CardPoint() <= 3)
                .ToList();
            foreach (var card in targets)
            {
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;
        }
    }
}
