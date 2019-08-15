using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("24006")]//守桥巨魔
    public class BridgeTroll : CardEffect
    {//将对方半场上的1个灾厄效果移至另一排。
        public BridgeTroll(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var allenemylist = new List<RowPosition>() { RowPosition.EnemyRow1, RowPosition.EnemyRow2, RowPosition.EnemyRow3 };
            var rowlist = new List<RowPosition>();
            foreach (var row in allenemylist)
            {
                if (Game.GameRowEffect[AnotherPlayer][row.Mirror().MyRowToIndex()].RowStatus.IsHazard())
                {
                    rowlist.Add(row);
                }
            }
            if (rowlist.Count() == 0)
            {
                return 0;
            }
            //把row1的灾厄移动到row2
            var row1 = await Game.GetSelectRow(Card.PlayerIndex, Card, rowlist);
            var nowpositon = new List<RowPosition>() { row1 };
            var moverowstatus = Game.GameRowEffect[AnotherPlayer][row1.Mirror().MyRowToIndex()].RowStatus;
            //做差集得到可移动位置
            var movelist = allenemylist.Except(nowpositon).ToList();
            var row2 = await Game.GetSelectRow(Card.PlayerIndex, Card, movelist);
            await Game.GameRowEffect[AnotherPlayer][row1.Mirror().MyRowToIndex()].SetStatus<NoneStatus>();

            //没找到简单的实现办法
            if (moverowstatus == RowStatus.BitingFrost)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<BitingFrostStatus>();
                return 0;
            }
            if (moverowstatus == RowStatus.BloodMoon)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
                return 0;
            }
            if (moverowstatus == RowStatus.DragonDream)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<DragonDreamStatus>();
                return 0;
            }
            if (moverowstatus == RowStatus.ImpenetrableFog)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<ImpenetrableFogStatus>();
                return 0;
            }
            if (moverowstatus == RowStatus.KorathiHeatwave)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<KorathiHeatwaveStatus>();
                return 0;
            }
            if (moverowstatus == RowStatus.PitTrap)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<PitTrapStatus>();
                return 0;
            }
            if (moverowstatus == RowStatus.RaghNarRoog)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<RaghNarRoogStatus>();

                return 0;
            }

            if (moverowstatus == RowStatus.SkelligeStorm)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<SkelligeStormStatus>();
                return 0;
            }

            if (moverowstatus == RowStatus.TorrentialRain)
            {
                await Game.GameRowEffect[AnotherPlayer][row2.Mirror().MyRowToIndex()].SetStatus<TorrentialRainStatus>();
                return 0;
            }
            return 0;




        }
    }
}