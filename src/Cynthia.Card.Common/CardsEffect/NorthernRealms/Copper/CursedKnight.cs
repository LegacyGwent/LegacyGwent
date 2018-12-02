using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44010")]//被诅咒的骑士
	public class CursedKnight : CardEffect
	{//将1个友军“诅咒生物”单位变为自身的原始同名牌。 2点护甲。
		public CursedKnight(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}