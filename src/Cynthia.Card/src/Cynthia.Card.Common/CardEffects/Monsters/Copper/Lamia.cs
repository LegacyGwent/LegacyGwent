using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24024")]//女蛇妖
	public class Lamia : CardEffect
	{//对1个敌军单位造成4点伤害，若目标位于“血月”之下，则伤害变为7点。
		public Lamia(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}