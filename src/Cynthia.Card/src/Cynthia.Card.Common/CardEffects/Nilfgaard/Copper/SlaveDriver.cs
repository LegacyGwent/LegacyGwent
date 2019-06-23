using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33023")]//奴隶贩子
	public class SlaveDriver : CardEffect
	{//将一个友军单位的战力降至1点，并对一个敌军单位造成伤害，数值等同于该友方单位所失去的战力。
		public SlaveDriver(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}