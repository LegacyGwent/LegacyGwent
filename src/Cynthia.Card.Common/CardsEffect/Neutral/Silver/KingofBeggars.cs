using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("13002")]//乞丐王
    public class KingofBeggars : CardEffect
    {
        public KingofBeggars(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            //对方战力比我方高多少(算上单位自身)
            var point = Game.GetPlayersPoint(Game.AnotherPlayer(Card.PlayerIndex)) - Game.GetPlayersPoint(Card.PlayerIndex);
            if (point <= 0) return 0;
            if (Card.CardPoint() + point > 15) point = (15 - Card.CardPoint());
            await Card.Effect.Strengthen(point);
            return 0;
        }
    }
}