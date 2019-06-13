using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32003")]//瓦提尔·德·李道克斯
    public class VattierDeRideaux : CardEffect
    {//揭示最多2张己方手牌，再随机揭示相同数量的对方卡牌
        public VattierDeRideaux(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var list = Game.PlayersHandCard[Card.PlayerIndex].Where(x => !x.Status.IsReveal).ToList();
            var cards = await Game.GetSelectMenuCards(Card.PlayerIndex, list, 2, isCanOver: true);
            foreach (var card in cards)
            {
                await card.Effect.Reveal(Card);
                if (Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)].Any(x => !x.Status.IsReveal))
                {
                    await Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)]
                        .Where(x => !x.Status.IsReveal)
                        .Mess().First().Effect.Reveal(Card);
                }
            }
            return 0;
        }
    }
}