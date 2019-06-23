using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43008")]//文森特·梅斯
	public class VincentMeis : CardEffect
	{//摧毁所有单位的护甲，获得被摧毁护甲数值一半的增益。
		public VincentMeis(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}