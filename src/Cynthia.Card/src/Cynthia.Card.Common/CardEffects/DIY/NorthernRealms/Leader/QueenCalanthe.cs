using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70179")]//卡兰瑟女王 Queen Calanthe
    public class QueenCalanthe : CardEffect
    {
        public QueenCalanthe(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var selectedAllies = await Game.GetSelectPlaceCards(
                Card,
                filter: target =>
                    target != Card &&
                    !target.Status.IsSpying &&
                    (target.Status.Group == Group.Copper || target.Status.Group == Group.Silver),
                selectMode: SelectModeType.MyRow);
            if (!selectedAllies.Any())
            {
                return 0;
            }

            var target = selectedAllies.Single();
            var boost = Math.Max(0, target.Status.HealthStatus);
            var armor = Math.Max(0, target.Status.Armor);

            // Drain here means consuming the current positive boost and armor as
            // power. Do not route this through Damage/Drain: Shield and row damage
            // modifiers must not change how much is consumed.
            if (boost > 0)
            {
                await target.Effect.Reset(Card);
            }
            if (armor > 0)
            {
                await Game.SendEvent(new AfterCardSubArmor(target, armor, Card));
                target.Status.Armor = 0;
                await Game.ShowSetCard(target);
            }
            if (boost + armor > 0)
            {
                await Card.Effect.Boost(boost + armor, Card);
            }

            await Game.ShowCardMove(
                new CardLocation(
                    RowPosition.MyDeck,
                    RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count + 1)),
                target);

            var candidates = Game.PlayersDeck[PlayerIndex]
                .Where(deckCard =>
                    deckCard.CardInfo().CardType == CardType.Unit &&
                    (deckCard.Status.Group == Group.Copper || deckCard.Status.Group == Group.Silver))
                .ToList();
            if (!candidates.Any())
            {
                return 0;
            }

            var selectedDeckCards = await Game.GetSelectMenuCards(
                PlayerIndex,
                candidates,
                1,
                isCanOver: false);
            if (!selectedDeckCards.Any())
            {
                return 0;
            }

            await selectedDeckCards.Single().MoveToCardStayFirst();
            return 1;
        }
    }
}
