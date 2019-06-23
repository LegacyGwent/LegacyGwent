using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24009")]//孽鬼战士
	public class NekkerWarrior : CardEffect
	{//选择1个友军铜色单位，将2张它的同名牌加入牌组底部。
		public NekkerWarrior(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}