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
			
			var deckselectlist = Game.PlayersDeck[Card.PlayerIndex].Where(x =>(x.Status.Group == Group.Copper)).Mess(RNG).ToList();
			if (deckselectlist.Count() == 0)
            {
                return 0;
            }
            var selectList = Game.PlayersHandCard[PlayerIndex].ToList();
            var handresult = await Game.GetSelectMenuCards(PlayerIndex, selectList, 2, "选择交换张牌", isCanOver: true);
            int swapnum = handresult.ToList().Count();
            if (swapnum == 0)
            {
                return 0;
            }
			var swapdeckcard = await Game.GetSelectMenuCards(Card.PlayerIndex, deckselectlist, swapnum, isCanOver: false);


            foreach (var card in handresult)
            {
                await card.Effect.Swap();
            }

            foreach(var card in swapdeckcard)
			{
				var row = Game.RowToList(card.PlayerIndex, card.Status.CardRow);
            	await Game.LogicCardMove(card, row, 0);//将选中的卡移动到最上方
				await Game.PlayerDrawCard(card.PlayerIndex);
			}


			return 0 ;
		}
	}
}