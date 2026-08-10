using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("63008")]//大野猪
    public class GiantBoar : CardEffect
    {//随机摧毁1个友军单位，然后获得10点增益。
        public GiantBoar(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var candidates = Game.PlayersHandCard[PlayerIndex].Any()
                ? Game.PlayersHandCard[PlayerIndex].Where(x => x != Card).ToList()
                : Game.RowToList(PlayerIndex, Card.Status.CardRow).Where(x => x != Card).ToList();
            if (candidates.Count == 0)
            {
                return 0;
            }
            var target = candidates.Mess(Game.RNG).First();
            await target.Effect.ToCemetery(CardBreakEffectType.Scorch);
            await Card.Effect.Boost(10, Card);


            return 0;
        }
    }
}
