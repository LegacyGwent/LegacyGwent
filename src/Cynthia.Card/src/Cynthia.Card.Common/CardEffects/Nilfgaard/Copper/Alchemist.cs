using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34009")]//炼金术士
    public class Alchemist : CardEffect
    {//揭示2张牌。
        public Alchemist(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)]
                .Concat(Game.PlayersHandCard[Card.PlayerIndex])
                .Where(x => !x.Status.IsReveal).ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2, isEnemyBack: true);
            foreach (var card in cards)
            {
                await card.Effect.Reveal(Card);
            }
            return 0;
        }
    }
}