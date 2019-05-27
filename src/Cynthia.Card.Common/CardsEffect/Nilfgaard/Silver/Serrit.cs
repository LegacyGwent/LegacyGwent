using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33012")]//瑟瑞特
	public class Serrit : CardEffect
	{//对1个敌军单位造成7点伤害，或将对方1张被揭示的单位牌战力降为1点
		public Serrit(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}