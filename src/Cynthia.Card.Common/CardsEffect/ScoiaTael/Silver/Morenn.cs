using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53006")]//莫丽恩
	public class Morenn : CardEffect
	{//伏击：在下个单位从任意方手牌打出至对方半场时翻开，对它造成7点伤害。
		public Morenn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}