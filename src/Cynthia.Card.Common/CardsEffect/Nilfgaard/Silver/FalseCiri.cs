using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("63002")]//冒牌希里
    public class FalseCiri : CardEffect
    {
        public FalseCiri(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task OnTurnStart(int playerIndex)
        {
            //我方回合开始时,如果没有被锁定,并且是间谍的话
            if (Card.PlayerIndex == playerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Boost(1);
        }
        public override async Task OnPlayerPass(int playerIndex)
        {
            if (Card.PlayerIndex == playerIndex && !Card.Status.IsLock && Card.Status.IsSpying) await Card.Effect.Charm();
        }
        public override async Task OnCardDeath(GameCard taget)
        {
            if (taget == Card) Game.Debug("冒牌希里凉啦!");
        }
    }
}