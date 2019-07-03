using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53002")]//艾雷亚斯
	public class EleYas : CardEffect
	{//被抽到或被收回牌组时获得2点增益。
		public EleYas(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}