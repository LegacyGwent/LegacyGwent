using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("43017")]//萨宾娜·葛丽维希格
	public class SabrinaGlevissig : CardEffect
	{//间谍。 遗愿：将该排最弱单位的战力应用于全排单位。
		public SabrinaGlevissig(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}