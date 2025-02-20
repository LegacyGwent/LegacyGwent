using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70172")]//图尔赛克家族驯兽师
    public class Princess : CardEffect
    {// Spawn a Giant Bear. If there is a Wither on this row, spawn a Raging Bear instead.
        public Princess(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var witchercount = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && x.HasAllCategorie(Categorie.Witcher) && x != Card).ToList().Count();
            if (witchercount == 0) // if no witcher spawn a giant bear
            { 
                await Game.CreateCard("15010", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
                return 1;
            }
            // otherwise spawn a raging bear
            await Game.CreateCard("65002", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }
    }
}