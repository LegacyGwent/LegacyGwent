using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("33020")]//通敌
	public class Treason : CardEffect
	{//迫使2个相邻敌军单位互相对决。
		public Treason(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			return 0;
		}
	}
}