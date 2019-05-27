using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("13021")]//杜度
	public class Dudu : CardEffect
	{//复制一个敌军单位的战力。
		public Dudu(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var result = await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.EnemyRow);
			if(result.Count<=0) return 0;
			var point = result.Single().ToHealth().health;
			var myPoint = Card.ToHealth().health;
			if(point>myPoint)
				await Card.Effect.Boost(point-myPoint,Card);
			if(point<myPoint)
				await Card.Effect.Damage(myPoint-point,Card);
			return 0;
		}
	}
}