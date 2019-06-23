using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("15003")]//店店：法师
	public class ShupeMage : CardEffect
	{//派“店店”去班·阿德学院，见见那里的小伙子们。 抽1张牌；随机魅惑1个敌军单位；在对方三排随机生成一种灾厄；对1个敌军造成10点伤害，再对其相邻单位造成5点；从牌组打出1张铜色/银色“特殊”牌。
		public ShupeMage(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}