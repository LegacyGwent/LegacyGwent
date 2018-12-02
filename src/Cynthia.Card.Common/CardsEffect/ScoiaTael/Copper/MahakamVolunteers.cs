using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54026")]//玛哈坎志愿军
	public class MahakamVolunteers : CardEffect
	{//召唤所有同名牌。
		public MahakamVolunteers(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}