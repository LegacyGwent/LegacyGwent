using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("25009")]//满月
    public class FullMoon : CardEffect
    {//在己方单排降下恩泽。在回合开始时，使该排上1个随机“野兽”或“吸血鬼”单位获得2点增益。
        public FullMoon(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.My.GetRow());
            await Game.GameRowEffect[PlayerIndex][result.MyRowToIndex()]
                .SetStatus<FullMoonStatus>();
            return 0;
        }
    }
}