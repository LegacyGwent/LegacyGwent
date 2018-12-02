using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12014")]//特莉丝：蝴蝶咒语
	public class TrissButterflySpell : CardEffect
	{//回合结束时，使其他最弱的友军单位获得1点增益。
		public TrissButterflySpell(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}