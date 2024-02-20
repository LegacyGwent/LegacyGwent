using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70145")]//奥莉安娜 Orianna
    public class Orianna : CardEffect
    {
        public Orianna(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var cards = Game.GetPlaceCards(PlayerIndex).Where(x => x.HasAllCategorie(Categorie.Vampire)).ToList();
            var card2 = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!card2.TrySingle(out var target))
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Drain(1,target);
            }
            return 0;
        }
    }
}
