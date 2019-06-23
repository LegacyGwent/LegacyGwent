using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24031")]//孽鬼
	public class Nekker : CardEffect
	{//若位于手牌、牌组或己方半场：友军单位发动吞噬时获得1点增益。 遗愿：召唤1张同名牌。
		public Nekker(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}