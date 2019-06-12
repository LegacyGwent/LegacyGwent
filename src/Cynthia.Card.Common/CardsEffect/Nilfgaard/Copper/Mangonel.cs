using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34016")]//射石机
    public class Mangonel : CardEffect
    {//对1个敌军随机单位造成2点伤害。己方每揭示1张牌，便再次触发此能力。
        public Mangonel(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var cards = Game.GetAllCard(Card.PlayerIndex)
                .Where(x => x.PlayerIndex != Card.PlayerIndex && x.Status.CardRow.IsOnPlace());
            if (cards.Count() == 0) return 0;
            await cards.Mess().First().Effect.Damage(2, Card);
            return 0;
        }

        public override async Task OnCardReveal(GameCard taget, GameCard soure = null)
        {
            if (soure == null || soure.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;
            await CardPlayEffect(Card.Status.IsSpying);
        }
    }
}