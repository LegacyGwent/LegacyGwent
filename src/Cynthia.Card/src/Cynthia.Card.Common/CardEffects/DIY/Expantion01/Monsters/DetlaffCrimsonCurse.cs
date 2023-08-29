using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System;

namespace Cynthia.Card
{
    [CardEffectId("70102")]//狄拉夫：猩红诅咒 DetlaffCrimsonCurse
    public class DetlaffCrimsonCurse : CardEffect
    {//放逐己方墓地中1张铜色“野兽”或“吸血鬼”，随后择一：在己方半场降下3排“满月”；或在对方半场降下3排“血月”。
        public DetlaffCrimsonCurse(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var list = Game.PlayersCemetery[Card.PlayerIndex].Where(x => x.HasAnyCategorie(Categorie.Beast,Categorie.Vampire)).ToList();
            //如果没有单位，什么都不做
            if (list.Count() < 3)
            {
                return 0;
            }
            //选择一个单位，如果不选，什么都不做
            var MoveTarget = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 3, isCanOver: false);
            if (MoveTarget.Count() < 3)
            {
                return 0;
            }
            foreach (var x in MoveTarget)
            {
                await x.Effect.Banish();
            }
            
            //选择选项,设置每个选项的名字和效果
            var switchCard = await Card.GetMenuSwitch
            (
                ("月夜降临", "在己方半场降下3排“满月”。"),
                ("百鬼夜行", "在对方半场降下3排“血月”。")
            );
            if (switchCard == 0)
            {
                await Game.GameRowEffect[PlayerIndex][RowPosition.MyRow1.MyRowToIndex()].SetStatus<FullMoonStatus>();
                await Game.GameRowEffect[PlayerIndex][RowPosition.MyRow2.MyRowToIndex()].SetStatus<FullMoonStatus>();
                await Game.GameRowEffect[PlayerIndex][RowPosition.MyRow3.MyRowToIndex()].SetStatus<FullMoonStatus>();
            }
            else if (switchCard == 1)
            {
                await Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow1.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
                await Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow2.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
                await Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow3.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
            }
            return 0;
        }
    }
}
