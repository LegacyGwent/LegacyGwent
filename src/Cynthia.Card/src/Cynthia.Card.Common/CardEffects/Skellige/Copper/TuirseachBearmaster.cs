using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("64033")]//图尔赛克家族驯兽师
    public class TuirseachBearmaster : CardEffect
    {//生成1头“熊”。
        public TuirseachBearmaster(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard("15010", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }
    }
}