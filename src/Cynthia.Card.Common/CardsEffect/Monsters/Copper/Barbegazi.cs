using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24025")]//须岩怪
	public class Barbegazi : CardEffect
	{//坚韧。 吞噬1个友军单位，获得其战力作为增益。
		public Barbegazi(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}