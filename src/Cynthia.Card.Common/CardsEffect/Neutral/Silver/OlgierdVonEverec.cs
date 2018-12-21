using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13015")]//欧吉尔德·伊佛瑞克
	public class OlgierdVonEverec : CardEffect
	{//遗愿：复活至原位。
		public OlgierdVonEverec(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task OnCardDeath(GameCard taget, CardLocation soure)
		{
			if(taget!=Card) return;
			await Card.Effect.Resurrect(soure);
			return;
		}
	}
}