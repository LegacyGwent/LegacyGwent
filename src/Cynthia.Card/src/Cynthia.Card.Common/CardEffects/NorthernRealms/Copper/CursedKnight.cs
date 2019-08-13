using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44010")]//被诅咒的骑士
    public class CursedKnight : CardEffect
    {//将1个友军“诅咒生物”单位变为自身的原始同名牌。 2点护甲。
        public CursedKnight(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.MyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Transform(CardId.CursedKnight, Card);
            return 0;
        }
        public override async Task CardDownEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(2, Card);
            return;
        }
    }

}