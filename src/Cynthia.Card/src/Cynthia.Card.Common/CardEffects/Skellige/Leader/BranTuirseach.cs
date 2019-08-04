using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("61004")]//布兰王
    public class BranTuirseach : CardEffect
    {//从牌组丢弃最多3张牌，其中的单位牌获得1点强化。
        public BranTuirseach(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //以下代码基于：本卡先强化再丢弃（莫克瓦格）
            var list = Game.PlayersDeck[Card.PlayerIndex].Mess(Game.RNG);
            if (list.Count() == 0)
            {
                return 0;
            }
            var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 3);

            if (result.Count() == 0)
            {
                return 0;
            }
            foreach (var card in result)
            {
                await card.Effect.Strengthen(1, Card);
                await card.Effect.Discard(Card);
            }
            return 0;
        }
    }
}