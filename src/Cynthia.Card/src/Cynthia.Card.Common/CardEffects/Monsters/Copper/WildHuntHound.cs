using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("24032")]//狂猎之犬
    public class WildHuntHound : CardEffect
    {//从牌组打出“刺骨冰霜”。
        public WildHuntHound(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var targetCards = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.CardId == CardId.BitingFrost).ToList();
            if (targetCards.Count == 0)
            {
                return 0;
            }
            await targetCards.First().MoveToCardStayFirst();
            return 0;
        }
    }
}