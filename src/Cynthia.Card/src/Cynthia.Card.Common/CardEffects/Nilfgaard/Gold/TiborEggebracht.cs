using System.Text.RegularExpressions;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("32005")]//蒂博尔·艾格布拉杰
    public class TiborEggebracht : CardEffect
    {//休战：获得15点增益，随后对方抽1张铜色牌并揭示它
        public TiborEggebracht(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            if (Game.IsPlayersPass[AnotherPlayer]) return 0;
            await Boost(15, Card);
            var drawCards = await Game.PlayerDrawCard(AnotherPlayer, filter: x => x.Status.Group == Group.Copper);
            foreach (var card in drawCards)
            {
                await card.Effect.Reveal(Card);
            }
            return 0;
        }
    }
}