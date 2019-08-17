using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12042")]//矮人符文剑
    public class Sihil : CardEffect
    {//择一：对所有战力为“奇数”的敌军单位造成3点伤害；对所有战力为“偶数”的敌军单位造成3点伤害；或从牌组随机打出1个铜色/银色单位。均衡分裂装备https://www.bilibili.com/video/av17561142?from=search&seid=13434231812588843367 2215
        public Sihil(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var switchCard = await Card.GetMenuSwitch(("寄生之缚", "对所有战力为“奇数”的敌军单位造成3点伤害。"), ("寄生之缚", "对所有战力为“偶数”的敌军单位造成3点伤害。"), ("低语", "从牌组随机打出1个铜色/银色单位。"));
        }
    }
}