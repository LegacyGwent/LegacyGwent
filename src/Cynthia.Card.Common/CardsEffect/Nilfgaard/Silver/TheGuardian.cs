using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("33003")]//魔像守卫
    public class TheGuardian : CardEffect
    {
        public TheGuardian(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            await Game.CreatCard("35001",Game.AnotherPlayer(Card.PlayerIndex),
            new CardLocation(RowPosition.MyDeck,0));
            return 0;
        }
    }
}