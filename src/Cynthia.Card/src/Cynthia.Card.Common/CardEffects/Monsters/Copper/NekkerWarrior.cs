using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("24009")]//孽鬼战士
	public class NekkerWarrior : CardEffect
	{//选择1个友军铜色单位，将2张它的同名牌加入牌组底部。
		public NekkerWarrior(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			//选择1个友军铜色单位
            var result = (await Game.GetSelectPlaceCards(Card, selectMode:SelectModeType.MyRow));

            //将2张它的同名牌加入牌组底部
            
            for(var i = 0; i < 2; i++)
            {
            	await Game.CreateCard(result.Single().CardId, Card.PlayerIndex, 
                    new CardLocation(RowPosition.MyDeck, Game.PlayersDeck[playerIndex].Count));
            }
            return 0;
		}
	}
}