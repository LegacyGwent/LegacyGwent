using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("61002")]//克拉茨·奎特
	public class CrachAnCraite : CardEffect
	{//使牌组中最强的非间谍铜色/银色单位牌获得2点强化，随后打出。
		public CrachAnCraite(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}