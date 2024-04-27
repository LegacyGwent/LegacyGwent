using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("70154")]//爱丽丝：庄园幽影
	public class IrisShade : CardEffect
	{//休战：为双方手牌各添加2张爱丽丝的伙伴。
		public IrisShade(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
            if (Game.IsPlayersPass[AnotherPlayer])
            {
                return 0;
            }
			await Game.CreateCardAtEnd("13004", PlayerIndex, RowPosition.MyHand);
			await Game.CreateCardAtEnd("13004", PlayerIndex, RowPosition.MyHand);
			await Game.CreateCardAtEnd("13004", AnotherPlayer, RowPosition.MyHand);
			await Game.CreateCardAtEnd("13004", AnotherPlayer, RowPosition.MyHand);
			return 0;
		}
	}
}
