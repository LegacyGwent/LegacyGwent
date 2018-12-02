using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("24015")]//狂猎长船
	public class WildHuntDrakkar : CardEffect
	{//使所有友军“狂猎”单位获得1点增益。 后续出现的其他友军“狂猎”单位也将获得1点增益。
		public WildHuntDrakkar(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}