using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12038")]//哈马维的梦境
	public class HanmarvynSDream : CardEffect
	{//生成对方墓场中1张非领袖金色单位牌的原始同名，并使其获得2点增益。
		public HanmarvynSDream(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}