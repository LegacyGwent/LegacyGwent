using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("44024")]//科德温亡魂
	public class KaedweniRevenant : CardEffect
	{//己方打出下一张“法术”或“道具”牌时，在所在排生成1张自身的佚亡原始同名牌。 1点护甲。
		public KaedweniRevenant(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}