using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24012")]//叉尾龙
	public class Forktail : CardEffect
	{//吞噬2个友军单位，并获得其战力的增益。
		public Forktail(IGwentServerGame game, GameCard card) : base(game, card){}
		public override async Task<int> CardPlayEffect(bool isSpying)
		{
			var result = await Game.GetSelectPlaceCards(Card, 2 , selectMode: SelectModeType.MyRow);
			foreach(var card in result)
			{
				await Consume(card);
			}
			return 0;
		}
	}
}
