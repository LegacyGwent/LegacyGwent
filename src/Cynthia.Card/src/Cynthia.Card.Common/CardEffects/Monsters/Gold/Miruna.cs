using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("22011")]//米卢娜
	public class Miruna : CardEffect
	{//2回合后的回合开始时：魅惑对方同排最强的单位。
		public Miruna(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}