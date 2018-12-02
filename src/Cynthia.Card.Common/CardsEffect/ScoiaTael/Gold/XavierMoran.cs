using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("52003")]//泽维尔·莫兰
	public class XavierMoran : CardEffect
	{//增益自身等同于最后打出的非同名“矮人”单位牌的初始战力。
		public XavierMoran(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}