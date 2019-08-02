using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("61003")]//埃斯特·图尔赛克
	public class EistTuirseach : CardEffect
	{//生成1个铜色“图尔赛克家族”的“士兵”单位。
		public EistTuirseach(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{	//图尔赛克家族，士兵，（铜单位）
            return await Card.CreateAndMoveStay(GwentMap.GetCreateCardsId(x => x.Group == Group.Copper&&x.CardInfo().CardType == CardType.Unit&&x.HasAllCategorie(Categorie.ClanTuirseach,Categorie.Soldier),RNG).ToList()
		}
	}
}