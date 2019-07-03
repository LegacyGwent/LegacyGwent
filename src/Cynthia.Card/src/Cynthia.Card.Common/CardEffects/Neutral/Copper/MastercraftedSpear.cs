using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("14024")]//精良的长矛
	public class MastercraftedSpear : CardEffect
	{//造成等同于己方手牌中1个铜色/银色单位的基础战力的伤害。
		public MastercraftedSpear(GameCard card) : base(card){}
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
			await result.Single().Effect.Damage(point,Card);
			return 0;
		}
	}
}