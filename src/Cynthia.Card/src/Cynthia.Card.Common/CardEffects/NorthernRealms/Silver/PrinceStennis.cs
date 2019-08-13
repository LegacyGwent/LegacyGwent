using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43016")]//斯坦尼斯王子
    public class PrinceStennis : CardEffect
    {//从牌组随机打出1张铜色/银色非间谍单位牌，使其获得5点护甲。 3点护甲。
        public PrinceStennis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(3, Card);
            var list = Game.PlayersDeck[Card.PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow && (x.Status.Group == Group.Silver || x.Status.Group == Group.Copper) && (x.CardInfo().CardType == CardType.Unit));
            if (!list.ToList().TryMessOne(out var target, Game.RNG))
            {
                return 0;
            }
            await target.Effect.Armor(5, Card);
            await target.MoveToCardStayFirst();

            return 1;
        }

    }
}