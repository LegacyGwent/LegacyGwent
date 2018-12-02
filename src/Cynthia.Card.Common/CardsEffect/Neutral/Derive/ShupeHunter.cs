using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("15002")]//店店：猎人
	public class ShupeHunter : CardEffect
	{//派“店店”去多尔·布雷坦纳的森林。 造成15点伤害；对一个敌军随机单位造成2点伤害，连续8次；重新打出1个铜色/银色单位，并使它获得5点增益；从牌组打出1张铜色/银色单位牌；移除己方半场的所有“灾厄”效果，并使友军单位获得1点增益。
		public ShupeHunter(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}