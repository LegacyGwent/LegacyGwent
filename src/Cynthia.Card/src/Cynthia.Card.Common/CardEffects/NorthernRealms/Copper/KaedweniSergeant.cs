using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44003")]//科德温中士
    public class KaedweniSergeant : CardEffect
    {//移除所在排的灾厄。 3点护甲。 操控。
        public KaedweniSergeant(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.Armor(3, Card);
            if (Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].RowStatus.IsHazard())
                await Game.GameRowEffect[PlayerIndex][Card.Status.CardRow.MyRowToIndex()].SetStatus<NoneStatus>();
            return 0;
        }
    }
}