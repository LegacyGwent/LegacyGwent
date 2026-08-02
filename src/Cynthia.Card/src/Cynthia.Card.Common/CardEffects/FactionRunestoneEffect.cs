using System;
using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    public abstract class FactionRunestoneEffect : CardEffect
    {
        protected FactionRunestoneEffect(GameCard card) : base(card) { }

        protected abstract Faction RunestoneFaction { get; }

        public override async Task<int> CardUseEffect()
        {
            var myPoint = Game.GetPlayersPoint(PlayerIndex);
            var enemyPoint = Game.GetPlayersPoint(AnotherPlayer);
            if (myPoint == enemyPoint) return 0;

            var parity = myPoint < enemyPoint ? 0 : 1;
            var candidates = GwentMap.GetGenerateCardsId(
                card => card.Faction == RunestoneFaction &&
                    card.Is(Group.Copper, CardType.Unit) &&
                    !card.HasAnyCategorie(Categorie.Agent) &&
                    Math.Abs(card.Strength % 2) == parity,
                Card.GetMyBaseDeck().Select(card => card.CardId));
            return await Card.CreateAndMoveStay(candidates.ToList());
        }
    }
}
