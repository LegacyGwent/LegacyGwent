using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("32011")]//雷索：弑王者
	public class LethoKingslayer : CardEffect
	{//择一：摧毁1名敌军领袖，自身获得5点增益；或从牌组打出1张铜色/银色“谋略”牌。
		public LethoKingslayer(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}