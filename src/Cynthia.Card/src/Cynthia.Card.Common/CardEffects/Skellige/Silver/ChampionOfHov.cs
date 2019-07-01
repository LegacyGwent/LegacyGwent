using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("63009")]//至尊冠军
	public class ChampionOfHov : CardEffect
	{//与1个敌军单位对决。
		public ChampionOfHov(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			return 0;
		}
	}
}