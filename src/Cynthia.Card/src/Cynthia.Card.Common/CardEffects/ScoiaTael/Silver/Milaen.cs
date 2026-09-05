using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("53014")]//麦莉
    public class Milaen : CardEffect
    {
        public Milaen(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var rowIndex = await Game.GetSelectRow(Card.PlayerIndex, Card,
                TurnType.Enemy.GetRow());
            var damage = 6 + Game.GetPlaceCards(PlayerIndex, isHasConceal: true)
                .Count(card => card.Status.Type == CardType.Unit &&
                               card.Status.Conceal &&
                               card.HasAllCategorie(Categorie.Ambush));
            var row = Game.RowToList(Card.PlayerIndex, rowIndex).ToList();
            if (row.Count <= 0)
                return 0;
            var cardLeft = row.First();
            var cardRight = row.Last();
            if (cardLeft.IsAliveOnPlance() && !cardLeft.Status.Conceal)
                await cardLeft.Effect.Damage(damage, Card);
            if (cardLeft == cardRight) return 0;
            if (cardRight.IsAliveOnPlance() && !cardRight.Status.Conceal)
                await cardRight.Effect.Damage(damage, Card);
            return 0;
        }
    }
}
