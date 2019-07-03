using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13035")]//史凯利格风暴
    public class SkelligeStorm : CardEffect
    {//在对方单排降下灾厄。回合开始时，对所在排最左侧的单位各造成2、1、1点伤害。
        public SkelligeStorm(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card,
                TurnType.Enemy.GetRow());
            // await Game.ApplyWeather(Card.PlayerIndex,result,RowStatus.SkelligeStorm);
            await Game.GameRowEffect[Game.AnotherPlayer(Card.PlayerIndex)][result.Mirror().MyRowToIndex()]
                .SetStatus<SkelligeStormStatus>();
            return 0;
        }
    }
}