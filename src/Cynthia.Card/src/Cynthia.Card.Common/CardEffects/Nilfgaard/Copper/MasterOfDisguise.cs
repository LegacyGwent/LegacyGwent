using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34006")]//伪装大师
	public class MasterOfDisguise : CardEffect
	{//隐匿2张牌。
		public MasterOfDisguise(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}