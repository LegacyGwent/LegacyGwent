using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54003")]//维里赫德旅新兵
	public class VriheddNeophyte : CardEffect
	{//随机使手牌中2个单位获得1点增益。
		public VriheddNeophyte(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}