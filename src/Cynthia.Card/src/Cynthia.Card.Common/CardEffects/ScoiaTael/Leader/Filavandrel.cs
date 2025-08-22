using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using Alsein.Extensions;
using Cynthia.Card.Common.CardEffects.Neutral.Derive;

namespace Cynthia.Card
{	
	//"choose one:create a sliver neutral spell card,or create a sliver scoia'tael special card. "
	[CardEffectId("51003")]//菲拉凡德芮
	public class Filavandrel : Choose
	{
		public Filavandrel(GameCard card) : base(card)
		{
		}

		protected override async Task<int> UseMethodByChoice(int switchCard)
		{
			switch (switchCard)
			{
				case 1:
					return await FUNCTION1();
				case 2:
					return await FUNCTION2();
			}

			return 0;
		}
		protected override void RealInitDict()
		{
			methodDesDict = new Dictionary<int, string>()
			{
				{1, "Filavandrel_1_CreateNeutralSpell"},
				{2, "Filavandrel_2_CreateSTSpecial"}
			};
		}
		private async Task<int> FUNCTION1()
        { //create a sliver neutral spell card
			return await Card.CreateAndMoveStay(
		        GwentMap.GetCreateCardsId(
		            x => (x.Group == Group.Silver)&&(x.CardInfo().CardType == CardType.Special && x.Faction == Faction.Neutral && x.HasAnyCategorie(Categorie.Spell)),
		            RNG
		        )
		        .ToList() 
		    );
        }
		private async Task<int> FUNCTION2()
		{	//create a sliver scoia'tael special card
			return await Card.CreateAndMoveStay(
		        GwentMap.GetCreateCardsId(
		            x => (x.Group == Group.Silver)&&(x.CardInfo().CardType == CardType.Special && x.Faction == Faction.ScoiaTael),
		            RNG
		        )
		        .ToList() 
		    );
		}
	}
}