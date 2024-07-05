using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70098")] //维里赫德旅破坏者 VriheddSaboteur
    public class VriheddSaboteur : CardEffect
    {//lay a random item from your deck or choose and play a scoiatael item from your deck. Boost self by 3 if the item kills a card
        public VriheddSaboteur(GameCard card) : base(card){}
        private bool isplayturn = false;
        private bool isneedboost = false;
        // private bool isneedboost = false;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var switchCard = await Card.GetMenuSwitch(
               ("Random", "play a random item"),
               ("Scoiatael", "play a scoiatael item")
            );
            // play a random item from your deck
            isplayturn = true;        
            if (switchCard == 0)
            {
                var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Item) && x.CardInfo().CardType == CardType.Special)).ToList();
                if (list.Count() == 0) return 0;
                var item = list.Mess(RNG).First();
                await item.MoveToCardStayFirst();
                while (isneedboost)
                    await Boost(3, Card);
                return 1;
            }
            // choose and play a scoiatael item from your deck
            else if (switchCard == 1)
            {
                var list = Game.PlayersDeck[PlayerIndex].Where(x => ((x.Status.Group == Group.Copper) && x.Status.Categories.Contains(Categorie.Item) && x.CardInfo().CardType == CardType.Special && x.CardInfo().Faction == Faction.ScoiaTael)).ToList();
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, list.ToList(), 1, "");
                if (result.Count() == 0) return 0;
                var item = result.First();
                await item.MoveToCardStayFirst();
                return 1;
            }
            return 0;
        }
    }
}