using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("42005")]//约翰·纳塔利斯
	public class JohnNatalis : CardEffect
	{//从牌组打出1张铜色/银色“谋略”牌。
		public JohnNatalis(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}