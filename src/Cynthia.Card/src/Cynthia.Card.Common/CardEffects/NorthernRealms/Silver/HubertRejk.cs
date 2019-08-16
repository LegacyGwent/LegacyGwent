using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("43010")]//休伯特·雷亚克
    public class HubertRejk : CardEffect
    {//汲食牌组中所有单位的增益，作为战力。
        public HubertRejk(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            //只作用于我方卡组
            var deck = Game.PlayersDeck[PlayerIndex].Where(x => x.Status.HealthStatus >= 1).ToList();
            foreach (var card in deck)
            {
                await Card.Effect.Drain(card.Status.HealthStatus, card);
            }
            return 0;

        }
    }
}