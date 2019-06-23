using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24015")]//狂猎长船
	public class WildHuntDrakkar : CardEffect
	{//使所有友军“狂猎”单位获得1点增益。 后续出现的其他友军“狂猎”单位也将获得1点增益。
		public WildHuntDrakkar(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}