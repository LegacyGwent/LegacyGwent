using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12018")]//伊瓦拉夸克斯
	public class Ihuarraquax : CardEffect
	{//对自身造成5点伤害。 当前战力等同于基础战力时，在回合结束时对3个敌方随机单位造成7点伤害。
		public Ihuarraquax(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}