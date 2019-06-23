using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44016")]//亚甸槌击者
	public class AedirnianMauler : CardEffect
	{//对1个敌军造成4点伤害。
		public AedirnianMauler(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}