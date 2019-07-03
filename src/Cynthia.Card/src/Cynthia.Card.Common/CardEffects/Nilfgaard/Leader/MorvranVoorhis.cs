using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("31003")]//莫尔凡·符里斯
    public class MorvranVoorhis : CardEffect
    {//揭示最多4张牌。
        public MorvranVoorhis(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var list = Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)]
               .Concat(Game.PlayersHandCard[Card.PlayerIndex])
               .Where(x => !x.Status.IsReveal).ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 4, isEnemyBack: true);
            foreach (var card in cards)
            {
                await card.Effect.Reveal(Card);
            }
            return 0;
        }
    }
}