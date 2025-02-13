using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70172")]//图尔赛克家族驯兽师
    public class Princess : CardEffect
    {// Spawn a bear. If there is a wither on its row, spawn a raging bear instead.
        public Princess(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var witchercount = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow).IgnoreConcealAndDead().Where(x => x.Status.CardRow.IsOnPlace() && x.HasAllCategorie(Categorie.Witcher) && x != Card).ToList().Count();
            if (witchercount == 0)
            { 
                await Game.CreateCard("15010", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
                return 1;
            }
            await Game.CreateCard("65002", Card.PlayerIndex, new CardLocation(RowPosition.MyStay, 0));
            return 1;
        }
    }
}