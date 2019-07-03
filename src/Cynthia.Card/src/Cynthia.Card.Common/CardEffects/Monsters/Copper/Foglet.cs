using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24028")]//小雾妖
	public class Foglet : CardEffect
	{//每当在对方半场降下“蔽日浓雾”，便召唤1张同名牌至己方同排。
		public Foglet(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}