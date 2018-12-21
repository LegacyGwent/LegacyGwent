using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
	[CardEffectId("13011")]//卡罗“砍刀”凡瑞西
	public class Cleaver : CardEffect
	{//造成等同于手牌数量的伤害。
		public Cleaver(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var point = Game.PlayersHandCard[PlayerIndex].Count();
			var result = await Game.GetSelectPlaceCards(Card,selectMode:SelectModeType.EnemyRow);
			if(result.Count<=0) return 0;
			await result.Single().Effect.Damage(point,Card);
			return 0;
		}
	}
}