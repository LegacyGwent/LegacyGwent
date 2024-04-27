using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70099")]//斯瓦勃洛 Svalblod
    public class Svalblod : CardEffect
    {//对牌组中的所有单位牌造成2点伤害，随后，随后强化2点战力。
        public Svalblod(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow).FilterCards(filter: x => x != Card).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Damage(2, Card);
                await card.Effect.Strengthen(2, Card);
            }
            return 0;
        }
    }
}
