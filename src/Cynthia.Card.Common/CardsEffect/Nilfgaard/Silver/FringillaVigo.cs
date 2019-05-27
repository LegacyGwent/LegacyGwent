using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33018")]//芙琳吉拉·薇歌
	public class FringillaVigo : CardEffect
	{//间谍。将左侧单位的战力复制给右侧单位。
		public FringillaVigo(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}