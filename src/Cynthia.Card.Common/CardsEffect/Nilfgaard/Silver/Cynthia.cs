using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("33013")]//辛西亚
	public class Cynthia : CardEffect
	{//揭示对方手牌中最强的单位牌，并获得等同于其战力的增益。
		public Cynthia(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}