using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13030")]//蝎尾狮毒液
	public class ManticoreVenom : CardEffect
	{//造成13点伤害。
		public ManticoreVenom(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Damage(13,Card,BulletType.RedLight);
			return 0;
		}
	}
}