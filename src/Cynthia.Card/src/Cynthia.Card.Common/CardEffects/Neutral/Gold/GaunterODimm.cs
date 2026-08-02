using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12020")]//刚特·欧迪姆
    public class GaunterODimm : CardEffect
    {//发牌员随机创造一张单位牌，你猜测其战力是大于、等于或小于6。如果你猜对了打出该牌。
        public GaunterODimm(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var target = GwentMap.GetCards().Where(x => (x.Group != Group.Leader) && x.CardInfo().CardType == CardType.Unit).Mess(RNG).First();
            var switchCard = await Card.GetMenuSwitch(("猜疑", "GaunterODimm_1_LowerThanSix"), ("警告", "GaunterODimm_2_EqualToSix"), ("贪婪", "GaunterODimm_3_HigherThanSix"));
            int juggnum = target.Strength == 6 ? 6 : (target.Strength > 6 ? 7 : 5);
            if (switchCard != juggnum - 5)
            {
                return 0;
            }
            await Game.CreateCard(target.CardId, PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }
    }
}
