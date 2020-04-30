using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52001")]//萨琪亚
	public class Saskia : CardEffect
	{//用最多2张牌交换同等数量的铜色牌。
		public Saskia(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			
			var deckselectlist = Game.PlayersDeck[Card.PlayerIndex].Mess(RNG).Where(x =>(x.Status.Group == Group.Copper));
			var deckCount = deckselectlist.Count();
			if (deckCount == 0)
            {
                return 0;
            }
            var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
            var handresult = await Game.GetSelectMenuCards(PlayerIndex, selectList, deckCount<2?deckCount:2, "选择交换张牌", isCanOver: true);
            int swapnum = handresult.ToList().Count();
            if (swapnum == 0)
            {
                return 0;
            }
            foreach (var card in handresult)
            {
                await card.Effect.Swap();
            }
			var swapdeckcard = await Game.GetSelectMenuCards(Card.PlayerIndex, deckselectlist.ToList(), swapnum, isCanOver: false);



            foreach(var card in swapdeckcard)
			{
				var row = Game.RowToList(card.PlayerIndex, card.Status.CardRow);
				await Game.PlayerDrawCard(card.PlayerIndex,1,x=>x==card);
			}


			return 0 ;
		}
	}
}