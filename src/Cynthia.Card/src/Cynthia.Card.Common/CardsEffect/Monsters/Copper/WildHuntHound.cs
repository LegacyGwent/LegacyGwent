using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24032")]//狂猎之犬
	public class WildHuntHound : CardEffect
	{//从牌组打出“刺骨冰霜”。
		public WildHuntHound(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}