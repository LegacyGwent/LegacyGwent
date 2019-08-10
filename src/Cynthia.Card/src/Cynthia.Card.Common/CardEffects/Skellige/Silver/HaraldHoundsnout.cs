using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Alsein.Extensions.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63012")]//哈罗德·霍兹诺特
    public class HaraldHoundsnout : CardEffect
    {//生成“威尔弗雷德”，“威尔海姆”，“威尔玛”。
        public HaraldHoundsnout(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(CardId.Wilfred, PlayerIndex, Card.GetLocation());
            await Game.CreateCard(CardId.Wilhelm, PlayerIndex, Card.GetLocation() + 1);
            //对面同排同序号生成
            await Game.CreateCard(CardId.Wilmar, PlayerIndex, Card.GetLocation().Mirror());
            return 0;
        }
    }
}