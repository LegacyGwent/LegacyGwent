using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12011")]//丹德里恩：虚妄荣光
	public class DandelionVainglory : CardEffect
	{//己方起始牌组中每有1张“杰洛特”、“叶奈法”、“特莉丝”或“卓尔坦”牌，便获得3点增益。
		public DandelionVainglory(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}