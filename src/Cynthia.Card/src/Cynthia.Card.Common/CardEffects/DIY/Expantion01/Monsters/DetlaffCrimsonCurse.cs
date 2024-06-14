using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{
    [CardEffectId("70102")] //狄拉夫：猩红诅咒 DetlaffCrimsonCurse
    public class DetlaffCrimsonCurse : Choose
    {//放逐己方墓地中1张铜色“野兽”或“吸血鬼”，随后择一：在己方半场降下3排“满月”；或在对方半场降下3排“血月”。
        public DetlaffCrimsonCurse(GameCard card) : base(card)
        {
        }

        protected override async Task<int> UseMethodByChoice(int switchCard)
        {
            var list = Game.PlayersCemetery[Card.PlayerIndex].Where(x => x.HasAnyCategorie(Categorie.Beast, Categorie.Vampire)).ToList();
            //如果没有单位，什么都不做
            if (list.Count < 3)
            {
                return 0;
            }
            //选择一个单位，如果不选，什么都不做
            var MoveTarget = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 3, isCanOver: false);
            if (MoveTarget.Count < 3)
            {
                return 0;
            }
            foreach (var x in MoveTarget)
            {
                await x.Effect.Banish();
            }

            switch (switchCard)
            {
                case 1:
                    return await FUNCTION1();
                case 2:
                    return await FUNCTION2();
                default:
                    return 0;
            }
        }

        protected override void RealInitDict()
        {
            methodDesDict = new Dictionary<int, string>()
            {
                {1, "DetlaffCrimsonCurse_1_Boon"},
                {2, "DetlaffCrimsonCurse_2_Hazard"}
            };
        }

        private async Task<int> FUNCTION1()
        {
            await Game.GameRowEffect[PlayerIndex][RowPosition.MyRow1.MyRowToIndex()].SetStatus<FullMoonStatus>();
            await Game.GameRowEffect[PlayerIndex][RowPosition.MyRow2.MyRowToIndex()].SetStatus<FullMoonStatus>();
            await Game.GameRowEffect[PlayerIndex][RowPosition.MyRow3.MyRowToIndex()].SetStatus<FullMoonStatus>();
            return 0;
        }

        private async Task<int> FUNCTION2()
        {
            await Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow1.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
            await Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow2.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
            await Game.GameRowEffect[AnotherPlayer][RowPosition.EnemyRow3.Mirror().MyRowToIndex()].SetStatus<BloodMoonStatus>();
            return 0;
        }
    }
}
