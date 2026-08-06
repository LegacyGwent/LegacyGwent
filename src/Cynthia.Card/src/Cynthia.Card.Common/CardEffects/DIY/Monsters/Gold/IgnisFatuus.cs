using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70192")]//伊格尼斯·法图斯
    public class IgnisFatuus : CardEffect, IHandlesEvent<BeforeCardDamage>
    {
        public IgnisFatuus(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Game.CreateCard(
                CardId.IgnisFatuus,
                PlayerIndex,
                Card.GetLocation(),
                status => status.IsDoomed = true);
            return 0;
        }

        public async Task HandleEvent(BeforeCardDamage @event)
        {
            if (Card.Status.CardRow.IsOnPlace() &&
                @event.Target.PlayerIndex == AnotherPlayer &&
                @event.DamageType == DamageType.ImpenetrableFog)
            {
                @event.IsWeaken = true;
            }

            await Task.CompletedTask;
        }
    }
}
