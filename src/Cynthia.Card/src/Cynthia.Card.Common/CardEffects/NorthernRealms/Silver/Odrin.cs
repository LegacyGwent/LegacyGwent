using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43009")]//欧德林
	public class Odrin : CardEffect
	{//回合开始时，移至随机排，并使同排所有友军单位获得1点增益。
		public Odrin(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}