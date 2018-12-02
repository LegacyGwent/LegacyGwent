using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("14016")]//斯丹莫福德的山崩术
	public class StammelfordSTremor : CardEffect
	{//对所有敌军单位造成1点伤害。
		public StammelfordSTremor(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}