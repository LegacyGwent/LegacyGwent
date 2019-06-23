using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24022")]//蜥蜴人战士
	public class VranWarrior : CardEffect
	{//吞噬右侧单位，获得其战力作为增益。 每2回合开始时，重复此能力。
		public VranWarrior(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}