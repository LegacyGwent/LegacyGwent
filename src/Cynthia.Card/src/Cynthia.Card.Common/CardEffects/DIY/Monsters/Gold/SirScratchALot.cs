using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;
using System.Collections.Generic;

namespace Cynthia.Card
{
    [CardEffectId("70177")]//挠挠先生
    public class SirScratchALot : CardEffect
    {//Deploy: strengthen all allied beasts by 1 wherever they are, then boost them by 1 if they are under full moon.
    // 部署：使所有野兽友方获得1点强化，无论他们身在何处，若他们处于满月下，则额外获得1点增益。
        public SirScratchALot(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            // strengthen all allied beasts by 1
            var cards = Game.PlayersHandCard[PlayerIndex].Concat(Game.GetPlaceCards(PlayerIndex)).Concat(Game.PlayersDeck[PlayerIndex]).FilterCards(filter: x => x.HasAllCategorie(Categorie.Beast)&&x!=Card);

            foreach(var card in cards)
            {
                await card.Effect.Strengthen(1, Card);
            }

            // boost them by 1 if they are under full moon.
            var tagetRows = Game.GameRowEffect[Card.PlayerIndex].Indexed()
                .Where(x => x.Value.RowStatus == RowStatus.FullMoon)
                .Select(x => x.Key);
            foreach (var rowIndex in tagetRows)
            {
                foreach (var beast in Game.PlayersPlace[Card.PlayerIndex][rowIndex].IgnoreConcealAndDead().FilterCards(filter: x => x.HasAllCategorie(Categorie.Beast)))
                {
                        await beast.Effect.Boost(1, Card);
                }
            }
            return 0;
        }
    }
}