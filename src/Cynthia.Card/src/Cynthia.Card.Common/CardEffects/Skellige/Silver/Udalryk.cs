using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63002")]//乌达瑞克
	public class Udalryk : CardEffect
	{//间谍、力竭。 检视牌组中2张牌。抽取1张，丢弃另1张。
		public Udalryk(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}