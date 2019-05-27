using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63001")]//茱塔·迪门
	public class JuttaAnDimun : CardEffect
	{//对自身造成1点伤害。
		public JuttaAnDimun(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}