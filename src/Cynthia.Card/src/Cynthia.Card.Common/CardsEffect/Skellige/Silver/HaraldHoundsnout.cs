using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63012")]//哈罗德·霍兹诺特
	public class HaraldHoundsnout : CardEffect
	{//生成“威尔弗雷德”，“威尔海姆”，“威尔玛”。
		public HaraldHoundsnout(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}