using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("53002")]//艾雷亚斯
	public class EleYas : CardEffect
	{//被抽到或被收回牌组时获得2点增益。
		public EleYas(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}