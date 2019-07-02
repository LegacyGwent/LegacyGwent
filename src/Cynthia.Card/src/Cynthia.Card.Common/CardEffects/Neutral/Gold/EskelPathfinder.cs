using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12016")]//艾斯卡尔：寻路者
	public class EskelPathfinder : CardEffect
	{//摧毁1个没有被增益的铜色/银色敌军单位。
		public EskelPathfinder(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            var cards = await Game.GetSelectPlaceCards(Card,filter:x=>(x.Status.Group==Group.Copper||x.Status.Group==Group.Silver)&&x.Status.HealthStatus<=0, selectMode: SelectModeType.EnemyRow);
            if (cards.Count == 0) return 0;
            await cards.Single().Effect.ToCemetery(CardBreakEffectType.Scorch);
			return 0;
		}
	}
}