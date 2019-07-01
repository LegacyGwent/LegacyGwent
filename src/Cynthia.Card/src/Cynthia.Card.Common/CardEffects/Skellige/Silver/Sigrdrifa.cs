using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63017")]//茜格德莉法
	public class Sigrdrifa : CardEffect
	{//复活1个铜色/银色“家族”单位。
		public Sigrdrifa(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}