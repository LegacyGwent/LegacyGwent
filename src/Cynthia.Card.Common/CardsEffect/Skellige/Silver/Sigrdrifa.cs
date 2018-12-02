using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("63017")]//茜格德莉法
	public class Sigrdrifa : CardEffect
	{//复活1个铜色/银色“家族”单位。
		public Sigrdrifa(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}