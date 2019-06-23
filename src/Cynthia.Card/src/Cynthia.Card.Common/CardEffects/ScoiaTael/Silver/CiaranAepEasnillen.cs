using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53003")]//席朗·依斯尼兰
	public class CiaranAepEasnillen : CardEffect
	{//改变1个单位的锁定状态，并把它移至其所在半场的同排。
		public CiaranAepEasnillen(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}