using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24013")]//巨棘魔树
	public class Archespore : CardEffect
	{//回合开始时移至随机排，对1个随机敌军单位造成1点伤害。 遗愿：对1个敌军随机单位造成4点伤害。
		public Archespore(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}