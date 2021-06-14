using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("130110")]//卡罗“砍刀”凡瑞西：晋升
	public class CleaverPro : CardEffect
	{//造成等同于手牌数量的伤害。对目标相邻单位造成溢出的伤害。
		public CleaverPro(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			var point = Game.PlayersHandCard[PlayerIndex].Count();
			var result = await Game.GetSelectPlaceCards(Card);
			if(result.Count<=0) 
            {
                return 0;
            }
            var card = result.Single();
            var org_point = card.CardPoint();
            await card.Effect.Damage(point,Card);
            if(org_point < point)
            {
                //如果左侧有单位且不是伏击卡
                var Ltaget = card.GetRangeCard(1, GetRangeType.HollowLeft);
                var Rtaget = card.GetRangeCard(1, GetRangeType.HollowRight);

                if (Ltaget.Count() != 0 && !Ltaget.Single().Status.Conceal)
                {
                    await Ltaget.Single().Effect.Damage(point-org_point,Card);
                }
                //如果右侧有单位且不是伏击卡
                if (Rtaget.Count() != 0 && !Rtaget.Single().Status.Conceal)
                {
                    await Rtaget.Single().Effect.Damage(point-org_point,Card);
                }
            }
			return 0;
		}
	}
}