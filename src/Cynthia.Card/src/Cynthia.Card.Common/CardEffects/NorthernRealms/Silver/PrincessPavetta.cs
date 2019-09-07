using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43015")]//帕薇塔公主
    public class PrincessPavetta : CardEffect
    {//将双方最弱的铜色/银色单位放回各自牌组。
        public PrincessPavetta(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //最弱定义为点数最弱（包括绿字）
            var mylist = Game.GetPlaceCards(Card.PlayerIndex).Where(x => x.Status.Group == Group.Silver || x.Status.Group == Group.Copper).WhereAllLowest();
            var enemylist = Game.GetPlaceCards(Game.AnotherPlayer(Card.PlayerIndex)).Where(x => x.Status.Group == Group.Silver || x.Status.Group == Group.Copper).WhereAllLowest();
            if (mylist.TryMessOne(out var mycard, Game.RNG))
            {
                mycard.Effect.Repair(true);
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), mycard, refreshPoint: true);

            }
            if (enemylist.TryMessOne(out var enemycard, Game.RNG))
            {
                enemycard.Effect.Repair(true);
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, RNG.Next(0, Game.PlayersDeck[Card.PlayerIndex].Count)), enemycard, refreshPoint: true);


            }

            return 0;
        }
    }
}