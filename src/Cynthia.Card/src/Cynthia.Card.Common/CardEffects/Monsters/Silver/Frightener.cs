using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23001")]//畏惧者
	public class Frightener : CardEffect
	{//间谍、力竭。 将1个敌军单位移至自身所在排，然后抽1张牌。
		public Frightener(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}