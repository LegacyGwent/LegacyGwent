using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("12014")]//特莉丝：蝴蝶咒语
    public class TrissButterflySpell : CardEffect
    {//回合结束时，使其他最弱的友军单位获得1点增益。
        public TrissButterflySpell(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task OnTurnOver(int playerIndex)
        {
            if (!(Card.Status.CardRow.IsOnPlace() && PlayerIndex == playerIndex)) return;
            var cards = Game.GetAllCard(PlayerIndex)
                .Where(x => x.Status.CardRow.IsOnPlace() && x.PlayerIndex == PlayerIndex && x != Card)
                .WhereAllLowest().ToList();
            foreach (var card in cards)
            {
                await card.Effect.Boost(1, Card);
            }
            return;
        }
    }
}