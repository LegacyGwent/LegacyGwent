using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14020")]//玛哈坎麦酒
	public class MahakamAle : CardEffect
	{//使己方每排的1个随机单位获得4点增益。
		public MahakamAle(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}