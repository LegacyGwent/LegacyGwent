using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22004")]//卡兰希尔
	public class CaranthirArFeiniel : CardEffect
	{//将1个敌军单位移至对方同排，并在此排降下“刺骨冰霜”。
		public CaranthirArFeiniel(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}