using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("34011")]//投尸机
	public class RotTosser : CardEffect
	{//在对方单排生成1个“牛尸”。
		public RotTosser(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			await Game.CreateCard(CardId.CowCarcass,Card.PlayerIndex,new CardLocation(RowPosition.MyStay,0));
			return 1;
		}
	}
}