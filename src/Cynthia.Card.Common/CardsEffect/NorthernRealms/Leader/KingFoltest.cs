using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("41003")]//弗尔泰斯特国王
	public class KingFoltest : CardEffect
	{//使己方半场其他单位，以及手牌和牌组中的非间谍单位获得1点增益。 操控。
		public KingFoltest(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}