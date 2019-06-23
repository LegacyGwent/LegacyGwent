using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53001")]//亚伊文
	public class Yaevinn : CardEffect
	{//间谍、力竭。 抽1张“特殊”牌和单位牌。保留1张，放回另一张。
		public Yaevinn(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}