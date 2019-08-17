using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53021")]//玛哈坎号角
	public class MahakamHorn : CardEffect
	{//择一：创造1张铜色/银色“矮人”牌；或使1个单位获得7点强化。
		public MahakamHorn(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{

			var switchCard = await Card.GetMenuSwitch(("警告", "创造1张铜色/银色“矮人”牌"), ("战斗的召唤", "使1个单位获得7点强化"));

			if (switchCard == 0)
			{
				
				return await Card.CreateAndMoveStay(
				GwentMap.GetCreateCardsId(
				x => x.HasAnyCategorie(Categorie.Dwarf) &&
				(x.Group == Group.Copper || x.Group == Group.Silver),
				RNG)
			.ToList());
			}

			if (switchCard == 1)
			{
				var targets = await Game.GetSelectPlaceCards(Card, 1, selectMode: SelectModeType.MyRow);
                if (targets.Count() == 0)
                {
                    return 0;
                }
                //强化7
                foreach (var target in targets)
                {
                    await target.Effect.Strengthen(7, Card);
                }

                return 0;
			}


			return 0;
			

		}
	}
}