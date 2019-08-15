using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("24027")]//猫人
    public class Werecat : CardEffect
    {//对1个敌军单位造成5点伤害，随后对位于“血月”之下的所有敌军单位造成1点伤害。
        public Werecat(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(5, Card);
            //所有排
            var allmylist = new List<RowPosition>() { RowPosition.MyRow1, RowPosition.MyRow2, RowPosition.MyRow3 };
            var damagelist = new List<GameCard>();

            foreach (var row in allmylist)
            {
                if (Game.GameRowEffect[AnotherPlayer][row.MyRowToIndex()].RowStatus == RowStatus.BloodMoon)
                {
                    damagelist.Concat(Game.RowToList(AnotherPlayer, row).ToList());
                }
            }
            if (damagelist.Count() == 0)
            {
                return 0;
            }
            foreach (var card in damagelist)
            {
                await card.Effect.Damage(1, Card);
            }
            return 0;
        }
    }
}