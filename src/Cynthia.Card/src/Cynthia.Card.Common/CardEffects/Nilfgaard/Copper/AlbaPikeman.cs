using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34024")]//阿尔巴师枪兵
	public class AlbaPikeman : CardEffect
	{//召唤所有同名牌。
		public AlbaPikeman(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}