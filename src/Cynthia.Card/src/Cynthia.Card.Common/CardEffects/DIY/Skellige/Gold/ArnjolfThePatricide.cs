using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70082")]//背亲者恩约夫 ArnjolfthePatricide
    public class ArnjolfthePatricide : CardEffect
    {//摧毁己方所有战力不高于2的单位，随后摧毁敌方场上所有战力不高于2的单位。
        public ArnjolfthePatricide(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var alliedCards = Game.GetPlaceCards(PlayerIndex)
                .Where(x => x.CardPoint() <= 2 && x != Card)
                .ToList();
            foreach (var card in alliedCards)
            {
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }

            var enemyCards = Game.GetPlaceCards(AnotherPlayer)
                .Where(x => x.CardPoint() <= 2)
                .ToList();
            foreach (var card in enemyCards)
            {
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;
        }
    }
}
