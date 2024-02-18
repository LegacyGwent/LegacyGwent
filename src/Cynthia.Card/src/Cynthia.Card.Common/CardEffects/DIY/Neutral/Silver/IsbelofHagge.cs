using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70155")]//哈吉的伊斯贝尔 IsbelofHagge
    public class IsbelofHagge : CardEffect
    {//重置1个单位，若其为友军单位则重复1次。
        public IsbelofHagge(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 2; i++)
            {
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
                if (!selectList.TrySingle(out var target))
                {
                    return 0;
                }
                if (target.PlayerIndex == Card.PlayerIndex)
                {
                    await target.Effect.Reset(Card);
                }
            }       
            return 0;
        }
    }
}
