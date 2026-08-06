using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70168")]//食人魔战士
    public class OgreWarrior : CardEffect, IHandlesEvent<AfterTurnStart>
    {
        public OgreWarrior(GameCard card) : base(card) { }

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Card.Effect.SetCountdown(value: 3);
            return 0;
        }

        public async Task HandleEvent(AfterTurnStart @event)
        {
            if (@event.PlayerIndex != PlayerIndex ||
                !Card.Status.CardRow.IsOnPlace() ||
                !Card.Status.IsCountdown)
            {
                return;
            }

            await Card.Effect.SetCountdown(offset: -1);
            if (Card.Effect.Countdown > 0)
            {
                return;
            }

            Card.Status.IsCountdown = false;
            await Game.ShowSetCard(Card);

            var allies = Game.GetPlaceCards(PlayerIndex).ToList().IgnoreConcealAndDead().ToList();
            var enemies = Game.GetPlaceCards(AnotherPlayer).ToList().IgnoreConcealAndDead().ToList();
            var alliedHighest = allies.Count == 0 ? 0 : allies.Max(unit => unit.CardPoint());
            var enemyHighest = enemies.Count == 0 ? 0 : enemies.Max(unit => unit.CardPoint());
            if (alliedHighest >= enemyHighest)
            {
                await Card.Effect.Boost((Card.CardPoint() + 1) / 2, Card);
            }
        }
    }
}
