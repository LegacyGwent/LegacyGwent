using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64023")]//奎特家族战吼者
	public class AnCraiteWarcrier : CardEffect
	{//使1个友军单位获得自身一半战力的增益。
		public AnCraiteWarcrier(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}