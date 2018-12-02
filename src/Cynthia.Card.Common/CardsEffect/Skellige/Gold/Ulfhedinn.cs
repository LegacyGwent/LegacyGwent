using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("62009")]//乌弗海登
	public class Ulfhedinn : CardEffect
	{//对所有敌军单位造成1点伤害，已受伤单位则承受2点伤害。
		public Ulfhedinn(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}