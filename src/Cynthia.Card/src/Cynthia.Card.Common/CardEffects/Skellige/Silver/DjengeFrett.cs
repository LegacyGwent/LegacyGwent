using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63003")]//邓戈·费特
	public class DjengeFrett : CardEffect
	{//对2个友军单位造成1点伤害。每影响一个单位，便获得2点强化。
		public DjengeFrett(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}