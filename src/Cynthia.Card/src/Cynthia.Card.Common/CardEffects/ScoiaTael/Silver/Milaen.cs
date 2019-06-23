using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53014")]//麦莉
	public class Milaen : CardEffect
	{//选定一排，做左右两侧末端的单位各造成6点伤害。
		public Milaen(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}