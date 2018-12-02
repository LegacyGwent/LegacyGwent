using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54012")]//玛哈坎劫掠者
	public class MahakamMarauder : CardEffect
	{//战力改变时（被重置除外），获得2点增益。
		public MahakamMarauder(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}