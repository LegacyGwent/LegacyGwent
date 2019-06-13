using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("54015")]//矮人好斗分子
	public class DwarvenSkirmisher : CardEffect
	{//对1个敌军单位造成3点伤害。若没有摧毁目标，则获得3点增益。
		public DwarvenSkirmisher(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}