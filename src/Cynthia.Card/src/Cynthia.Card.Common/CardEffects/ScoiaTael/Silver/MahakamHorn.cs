using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("53021")]//玛哈坎号角
	public class MahakamHorn : CardEffect
	{//择一：生成1个己方起始牌组之外的铜色“矮人”单位；或使1个单位获得7点强化。
		public MahakamHorn(GameCard card) : base(card){}
		public override async Task<int> CardUseEffect()
		{

			var switchCard = await Card.GetMenuSwitch(("警告", "MahakamHorn_1_CreateDwarf"), ("战斗的召唤", "MahakamHorn_2_Strenghten"));

			if (switchCard == 0)
			{

				return await Card.CreateAndMoveStay(
				GwentMap.GetGenerateCardsId(
				x => x.Is(Group.Copper, CardType.Unit) &&
				x.HasAnyCategorie(Categorie.Dwarf),
				Card.GetMyBaseDeck().Select(x => x.CardId))
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
