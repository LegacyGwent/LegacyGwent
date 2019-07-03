using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12042")]//矮人符文剑
	public class Sihil : CardEffect
	{//择一：对所有战力为“奇数”的敌军单位造成3点伤害；对所有战力为“偶数”的敌军单位造成3点伤害；或从牌组随机打出1个铜色/银色单位。
		public Sihil(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}