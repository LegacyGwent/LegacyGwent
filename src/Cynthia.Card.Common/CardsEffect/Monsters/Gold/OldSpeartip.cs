using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("22003")]//老矛头
	public class OldSpeartip : CardEffect
	{//对最多5个敌军同排单位造成2点伤害。
		public OldSpeartip(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}