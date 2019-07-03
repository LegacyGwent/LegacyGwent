using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("13038")]//白霜
    public class WhiteFrost : CardEffect
    {//在对方相邻两排降下灾厄。回合开始时，对所在排最弱的单位造成2点伤害。
        public WhiteFrost(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var row = (await Game.GetSelectRow(PlayerIndex, Card, new[] { RowPosition.EnemyRow1, RowPosition.EnemyRow2 })).Mirror().MyRowToIndex();
            await Game.GameRowEffect[AnotherPlayer][row].SetStatus<BitingFrostStatus>();
            await Game.GameRowEffect[AnotherPlayer][row + 1].SetStatus<BitingFrostStatus>();
            return 0;
        }
    }
}