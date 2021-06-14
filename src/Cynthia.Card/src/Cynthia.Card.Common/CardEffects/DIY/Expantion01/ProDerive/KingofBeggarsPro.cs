using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("130020")]//乞丐王：晋升
    public class KingOfBeggarsPro : CardEffect
    {//获得强化，直至战力超过对方1点或最多到20点。
        public KingOfBeggarsPro(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            //对方战力比我方高多少(算上单位自身)
            var point = Game.GetPlayersPoint(Game.AnotherPlayer(Card.PlayerIndex)) - Game.GetPlayersPoint(Card.PlayerIndex) + 1;
            if (point <= 0) return 0;
            if (Card.CardPoint() + point > 20) point = (20 - Card.CardPoint());
            await Card.Effect.Strengthen(point, Card);
            return 0;
        }
    }
}