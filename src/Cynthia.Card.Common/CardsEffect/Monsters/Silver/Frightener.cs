using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("23001")]//畏惧者
	public class Frightener : CardEffect
	{//间谍、力竭。 将1个敌军单位移至自身所在排，然后抽1张牌。
		public Frightener(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}