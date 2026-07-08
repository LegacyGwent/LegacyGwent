using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70159")]//
    public class Crowmother : CardEffect
    {//
        public Crowmother(GameCard card) : base(card) { }
        // 生成2只乌鸦。复活所有战力不高于2的乌鸦。
        // Spawn 3 Crows. Resurrect all Crows with power equal to or less than 2. Doomed.
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            for (var i = 0; i < 2; i++)
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
            var cards = Game.PlayersCemetery[PlayerIndex].Where(x => x.Status.CardId == CardId.Crow && x.Status.Strength <= 2).ToList();
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
            }
            return 0;
        }
    }
}
