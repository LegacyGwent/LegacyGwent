using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24030")]//鹰身女妖
	public class Harpy : CardEffect
	{//每当1个友军“野兽”单位在己方回合被摧毁，便召唤1张同名牌。
		public Harpy(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}