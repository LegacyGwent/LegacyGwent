using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24009")]//孽鬼战士
	public class NekkerWarrior : CardEffect
	{//选择1个友军铜色单位，将2张它的同名牌加入牌组底部。
		public NekkerWarrior(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}