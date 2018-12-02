using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14010")]//佩特里的魔药
	public class PetriSPhilter : CardEffect
	{//使最多6个友军随机单位获得2点增益。
		public PetriSPhilter(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}