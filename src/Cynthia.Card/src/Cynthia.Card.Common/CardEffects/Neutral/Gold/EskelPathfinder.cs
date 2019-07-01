using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12016")]//艾斯卡尔：寻路者
	public class EskelPathfinder : CardEffect
	{//摧毁1个没有被增益的铜色/银色敌军单位。
		public EskelPathfinder(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}