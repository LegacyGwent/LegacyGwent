using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("54009")]//多尔·布雷坦纳射手
	public class DolBlathannaBowman : CardEffect
	{//对1个敌军单位造成2点伤害。 每当有敌军单位改变所在排别，便对其造成2点伤害。 自身移动时对1个敌军随机单位造成2点伤害。
		public DolBlathannaBowman(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}