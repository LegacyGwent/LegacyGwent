using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70001")]//昆恩护盾
    public class Quen : CardEffect
    {
        public Quen(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var unitHandCard = Game.PlayersHandCard[PlayerIndex]
                .Where(x => x.Status.Type == CardType.Unit &&
                    x.IsAnyGroup(Group.Copper, Group.Silver) &&
                    !x.Status.IsShield).ToList();
            if (unitHandCard.Count == 0) { return 0; }

            var targetcards = await Game.GetSelectMenuCards(PlayerIndex, unitHandCard, isCanOver: false);
            if (!targetcards.TrySingle(out var target)) { return 0; }

            var sameNameCards = Game.PlayersHandCard[PlayerIndex]
                .Concat(Game.PlayersDeck[PlayerIndex])
                .Where(x => x.CardInfo().CardId == target.CardInfo().CardId)
                .ToList();
            foreach (var card in sameNameCards)
            {
                card.Status.IsShield = true;
                await card.Effect.Boost(2, Card);
            }
            return 0;
        }
    }
}
