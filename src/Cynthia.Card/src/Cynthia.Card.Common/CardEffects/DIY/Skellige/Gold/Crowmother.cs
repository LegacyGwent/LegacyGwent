using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70159")]//
    public class Crowmother : CardEffect
    {//
        public Crowmother(GameCard card) : base(card) { }
        // 生成3只乌鸦。复活所有战力不高于2的乌鸦，并使其获得佚亡。
        // Spawn 3 Crows. Resurrect all Crows with power equal to or less than 2, and give them Doomed.
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 3; i++)
            {
                if(Game.RowToList(Card.PlayerIndex, Card.GetLocation().RowPosition).Count() < Game.RowMaxCount)
                {
                    await Game.CreateCard(CardId.Crow, PlayerIndex, Card.GetLocation() + 1);
                }
                else
                {
                    await Game.CreateCard(CardId.Crow, PlayerIndex, Game.GetRandomCanPlayLocation(PlayerIndex, true));
                }
            }
            var cards = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.CardId == CardId.Crow).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                
                if(Game.RowToList(Card.PlayerIndex, Card.GetLocation().RowPosition).Count() < Game.RowMaxCount)
                {
                    await card.Effect.Resurrect(Card.GetLocation() + 1, card);
                }
                else
                {
                    await card.Effect.Resurrect(Game.GetRandomCanPlayLocation(PlayerIndex, true), Card);
                }
                card.Status.IsDoomed = true;
            }
            return 0;
        }
    }
}
