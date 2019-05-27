using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14015")]//翼手龙鳞甲盾牌
	public class WyvernScaleShield : CardEffect
	{//使1个单位获得等同于手牌中1张铜色/银色单位牌基础战力的增益。
		public WyvernScaleShield(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardUseEffect()
		{
			var list = Game.PlayersHandCard[Card.PlayerIndex]
            	.Where(x => (x.Status.Group == Group.Copper || x.Status.Group==Group.Silver)&&x.CardInfo().CardType == CardType.Unit);
            //让玩家选择一张卡
            var result = await Game.GetSelectMenuCards
            	(Card.PlayerIndex, list.ToList(), 1);
            if (result.Count() == 0) return 0;
			var point = result.Single().Status.Strength;

			result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Boost(point,Card);
			return 0;
		}
	}
}