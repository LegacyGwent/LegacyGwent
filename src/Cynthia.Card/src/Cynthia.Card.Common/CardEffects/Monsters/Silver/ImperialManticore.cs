using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("23002")]//帝国蝎尾狮
	public class ImperialManticore : CardEffect
	{//没有特殊技能。
		public ImperialManticore(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}