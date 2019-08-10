using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("23016")]//阿巴亚
    public class Abaya : CardEffect
    {//生成“倾盆大雨”、“晴空”或“蟹蜘蛛毒液”。
        public Abaya(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            return await Card.CreateAndMoveStay(CardId.TorrentialRain, CardId.ClearSkies, CardId.ArachasVenom);
        }
    }
}