using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34025")]//帝国魔像
    public class ImperialGolem : CardEffect
    {//每当己方揭示1张对方手牌，便从牌组召唤1张同名牌。
        public ImperialGolem(IGwentServerGame game, GameCard card) : base(game, card) { }
        private static GameCard temp;
        public override async Task OnCardReveal(GameCard target, GameCard soure = null)
        {
            if (Card.Status.CardRow != RowPosition.MyDeck || temp == target || target.PlayerIndex == Card.PlayerIndex || soure == null || soure.PlayerIndex != Card.PlayerIndex) return;
            temp = target;
            await Card.Effect.Play(Game.GetRandomCanPlayLocation(Card.PlayerIndex));
        }
    }
}