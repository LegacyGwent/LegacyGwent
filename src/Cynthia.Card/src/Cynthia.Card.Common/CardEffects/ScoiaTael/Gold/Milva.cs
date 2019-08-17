using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
	[CardEffectId("52008")]//米尔瓦
	public class Milva : CardEffect
	{//将双方最强的铜色/银色单位收回各自牌组。
		public Milva(GameCard card) : base(card){}
		public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
		{
			//最强定义为点数最强（包括绿字）
            var mylist = Game.GetPlaceCards(Card.PlayerIndex).Where(x => x.Status.Group == Group.Silver || x.Status.Group == Group.Copper).WhereAllHighest();
            var enemylist = Game.GetPlaceCards(Game.AnotherPlayer(Card.PlayerIndex)).Where(x => x.Status.Group == Group.Silver || x.Status.Group == Group.Copper).WhereAllHighest();
            if (mylist.TryMessOne(out var mycard, Game.RNG))
            {
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), mycard,refreshPoint:true);

            }
            if (enemylist.TryMessOne(out var enemycard, Game.RNG))
            {
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), enemycard,refreshPoint:true);
            }

            return 0;
		}
	}
}