using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("55001")] //焚烧陷阱
    public class IncineratingTrap : CardEffect
    {
        //对同排除自身外所有单位造成2点伤害，并在回合结束时放逐自身。
        public IncineratingTrap(IGwentServerGame game, GameCard card) : base(game, card)
        {
        }

        private int damage = 2;

        public override async Task OnTurnOver(int playerIndex)
        {
            if (playerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsOnPlace()) return;
            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown > 0) return;
            var row = Game.RowToList(Card.PlayerIndex, Card.GetLocation().RowPosition).ToList();
            foreach (var it in row)
            {
                if (it != Card)
                {
                    await it.Effect.Damage(damage, Card);
                }
            }

            await Card.Effect.Banish();
        }
    }
}