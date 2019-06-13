using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43021")]//范德格里夫特之剑
	public class VandergriftSBlade : CardEffect
	{//择一：摧毁1个铜色/银色“诅咒单位”敌军单位；或造成9点伤害，放逐所摧毁的单位。
		public VandergriftSBlade(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}