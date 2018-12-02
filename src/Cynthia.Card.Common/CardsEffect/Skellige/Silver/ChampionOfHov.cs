using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("63009")]//至尊冠军
	public class ChampionOfHov : CardEffect
	{//与1个敌军单位对决。
		public ChampionOfHov(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			return 0;
		}
	}
}