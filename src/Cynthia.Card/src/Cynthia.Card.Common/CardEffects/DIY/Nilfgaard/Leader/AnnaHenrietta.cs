using System.Linq;
using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("70149")]//安娜·亨利叶塔 AnnaHenrietta
    public class AnnaHenrietta : CardEffect
    {
        public AnnaHenrietta(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersDeck[Card.PlayerIndex];
            var StrengthList = list.Select(x => (Strength: x.Status.Strength, card: x)).OrderByDescending(x => x.Strength);
            if (list.Count() == 0)
            {
                return 0;
            }
            for(var i = 0; i<StrengthList.Count();i++)
            {
                var move = StrengthList.Select(x => x.card).ToList();
                await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, 0), move[i]);
            }

            await Game.PlayersDeck[Card.PlayerIndex].ToList()[0].MoveToCardStayFirst();
            return 1;
        }
    }
}
