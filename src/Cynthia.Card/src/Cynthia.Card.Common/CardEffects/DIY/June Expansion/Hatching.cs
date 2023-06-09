using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70083")]//spawn 3 arachas hatchlings, increase to 4 if there is a hatching in the graveyard
    public class Hatching : CardEffect
    {
        public Hatching(GameCard card) : base(card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectRow(Card.PlayerIndex, Card, TurnType.My.GetRow());
            var row = Game.RowToList(Card.PlayerIndex, result);
            var cards = (Game.PlayersCemetery[Card.PlayerIndex].ToList().Where(x => x.Status.CardId == Card.Status.CardId).Count());
            if (cards == 0)
            {
                for (var i = 0; i < 3; i++)
                {
                    if (row.Count < Game.RowMaxCount)
                        await Game.CreateCard("25002", Card.PlayerIndex, new CardLocation(result, row.Count));
                }
                return 0;
            }
            else
            {
                for (var i = 0; i < 4; i++)
                {
                    if (row.Count < Game.RowMaxCount)
                        await Game.CreateCard("25002", Card.PlayerIndex, new CardLocation(result, row.Count));
                }
                return 0;
            }
        }
    }
}