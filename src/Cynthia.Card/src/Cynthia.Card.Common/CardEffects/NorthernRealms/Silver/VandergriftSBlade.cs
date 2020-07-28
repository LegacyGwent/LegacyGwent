using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43021")]//范德格里夫特之剑
    public class VandergriftSBlade : CardEffect
    {//择一：摧毁1个铜色/银色“诅咒生物”敌军单位；或造成10点伤害，放逐所摧毁的单位。
        public VandergriftSBlade(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            //卡牌描述来自https://www.bilibili.com/video/av17561142?from=search&seid=5862653946081547514 48：56
            var switchCard = await Card.GetMenuSwitch(("一刀两断", "摧毁1个铜色/银色“诅咒单位”敌军单位。"), ("凶暴重击", "造成10点伤害，放逐所摧毁的单位"));
            if (switchCard == 0)
            {
                var target = await Game.GetSelectPlaceCards(Card, 1, false,
                    x => x.Status.Categories.Contains(Categorie.Cursed) && (x.Status.Group == Group.Copper || x.Status.Group == Group.Silver));
                if (target.Count == 0)
                {
                    return 0;
                }
                await target.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
                return 0;
            }

            if (switchCard == 1)
            {
                var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.AllRow);
                if (!selectList.TrySingle(out var target))
                {
                    return 0;
                }
                await target.Effect.Damage(10, Card);
                //如果目标没死，结束
                if (!target.IsDead)
                {
                    return 0;
                }
                target.Status.IsDoomed = true;
                return 0;

            }
            return 0;
        }
    }
}