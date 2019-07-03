using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("62010")]//凯瑞丝：无所畏惧
	public class CerysFearless : CardEffect
	{//复活己方下张丢弃的单位牌。
		public CerysFearless(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}