using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13019")]//爱丽丝·伊佛瑞克
	public class IrisVonEverec : CardEffect
	{//间谍。 遗愿：使对面半场5个随机单位获得5点增益。
		public IrisVonEverec(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}