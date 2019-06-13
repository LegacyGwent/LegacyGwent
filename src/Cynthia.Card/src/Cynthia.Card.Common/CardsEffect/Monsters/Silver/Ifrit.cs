using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23005")]//伊夫利特
	public class Ifrit : CardEffect
	{//在右侧生成3个“次级伊夫利特”。
		public Ifrit(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}