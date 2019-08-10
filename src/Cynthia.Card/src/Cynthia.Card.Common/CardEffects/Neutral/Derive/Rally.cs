using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("15014")]//重整
    public class Rally : CardEffect
    {//从己方牌组打出1张随机铜色单位牌。
        public Rally(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var list = Game.PlayersDeck[Card.PlayerIndex]
            .Where(x => ((x.Status.Group == Group.Copper) &&//铜色
                    (x.CardInfo().CardType == CardType.Unit))).Mess(RNG).ToList();//单位牌
            if (list.Count() == 0) return 0;
            var moveCard = list.First();
            await moveCard.MoveToCardStayFirst();
            return 1;
        }
    }
}