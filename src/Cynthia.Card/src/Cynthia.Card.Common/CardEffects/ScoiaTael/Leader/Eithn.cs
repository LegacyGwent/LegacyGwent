using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("51002")]//艾思娜
	public class Eithn : CardEffect
	{//复活1张铜色/银色“特殊”牌。
		public Eithn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}