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


            await Game.CreateCard(CardId.ImperialManticore, PlayerIndex, new CardLocation(RowPosition.MyStay, 0), setting: ToConceal);
            var switchCard = await Card.GetMenuSwitch(("mistrust", "小于6."), ("caution", "等于6"), ("greed", "大于6"));
            int juggnum = GwentMap.CardMap[CardId.ImperialManticore].Strength == 6 ? 6 : (GwentMap.CardMap[CardId.ImperialManticore].Strength > 6 ? 7 : 5);
            await Game.PlayersStay[PlayerIndex][0].Effect.Reveal(Card);
            if (switchCard != juggnum - 5)
            {
                await Game.PlayersStay[PlayerIndex][0].Effect.Banish();
                return 0;
            }
            return 1;
        }

        private void ToConceal(CardStatus status)
        {
            status.Conceal = true;
        }
    }
}