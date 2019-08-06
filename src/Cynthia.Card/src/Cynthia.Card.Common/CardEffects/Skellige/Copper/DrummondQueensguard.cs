using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64025")]//德拉蒙家族女王卫队
    public class DrummondQueensguard : CardEffect
    {//复活所有同名牌。
        public DrummondQueensguard(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //https://www.bilibili.com/video/av31563995?from=search&seid=8170290168074850146
            //复活所有可复活的卡到本卡的右边
            var cards = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.CardId == Card.Status.CardId).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Resurrect(Card.GetLocation() + 1, Card);
            }
            return 0;
        }
    }
}