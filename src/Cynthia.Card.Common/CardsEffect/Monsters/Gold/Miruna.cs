using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("22011")]//米卢娜
	public class Miruna : CardEffect
	{//2回合后的回合开始时：魅惑对方同排最强的单位。
		public Miruna(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}