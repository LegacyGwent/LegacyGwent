using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("64020")]//奎特家族劫掠者
	public class AnCraiteMarauder : CardEffect
	{//造成4点伤害。若被复活，则造成6点伤害。
		public AnCraiteMarauder(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}