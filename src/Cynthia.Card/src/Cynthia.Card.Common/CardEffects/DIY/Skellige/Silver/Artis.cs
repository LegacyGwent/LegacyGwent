using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70089")]//亚提斯
    public class Artis : CardEffect
    {//在对方同排生成“巨熊祭品”，随后将1个友军单位转化为“斯瓦勃洛狂信者”。
        public Artis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(
                CardId.CultistOblation,
                AnotherPlayer,
                new CardLocation(Card.Status.CardRow, int.MaxValue),
                source: Card);

            var selectList = await Game.GetSelectPlaceCards(
                Card,
                selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }

            await target.Effect.Transform(CardId.SvalblodFanatic, Card, isForce: true);
            return 0;
        }
    }
}
