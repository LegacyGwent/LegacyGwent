using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("12030")]//狐妖：真身
	public class AguaraTrueForm : CardEffect
	{//不限阵营地创造1张铜色/银色“法术”牌。
		public AguaraTrueForm(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			var list = GwentMap.GetCards()
				.Where(x=>(x.Categories.Contains(Categorie.Spell))
						&&(x.Group == Group.Copper||x.Group == Group.Silver))
				.Mess().Take(3).Select(x=>x.CardId)
				.ToList();
			return await Card.CreateAndMoveStay(list);
		}
	}
}