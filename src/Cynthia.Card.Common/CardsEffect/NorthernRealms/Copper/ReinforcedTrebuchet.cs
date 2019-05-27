using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("44008")]//加强型投石机
	public class ReinforcedTrebuchet : CardEffect
	{//回合结束时，对1个敌军随机单位造成1点伤害。
		public ReinforcedTrebuchet(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}