using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("12021")]//杰洛特：阿尔德法印
	public class GeraltAard : CardEffect
	{//选择3个敌军单位各造成3点伤害，并将它们上移1排。
		public GeraltAard(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}