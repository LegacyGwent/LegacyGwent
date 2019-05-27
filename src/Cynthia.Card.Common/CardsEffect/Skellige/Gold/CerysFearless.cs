using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62010")]//凯瑞丝：无所畏惧
	public class CerysFearless : CardEffect
	{//复活己方下张丢弃的单位牌。
		public CerysFearless(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}