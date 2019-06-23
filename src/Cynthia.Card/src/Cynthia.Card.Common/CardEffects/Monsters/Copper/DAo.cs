using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24020")]//地灵
	public class DAo : CardEffect
	{//遗愿：在同排生成2个“次级地灵”。
		public DAo(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}