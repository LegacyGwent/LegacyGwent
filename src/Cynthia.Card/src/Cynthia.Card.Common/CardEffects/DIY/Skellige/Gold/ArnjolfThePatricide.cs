using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70082")]//背亲者恩约夫 ArnjolfthePatricide
    public class ArnjolfthePatricide : CardEffect
    {//摧毁场上所有战力低于3的单位。
        public ArnjolfthePatricide(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = Game.GetAllCard(Card.PlayerIndex, isHasConceal: true).Where(x => x.Status.CardRow.IsOnPlace() && x.CardPoint() < 3 && x != Card).ToList();
            foreach (var card in cards)
            {
                await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
            }
            return 0;
        }
    }
}
