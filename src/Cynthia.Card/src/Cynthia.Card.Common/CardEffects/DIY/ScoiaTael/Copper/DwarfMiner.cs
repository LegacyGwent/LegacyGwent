using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70097")]//矮人矿工
    public class DwarfMiner : CardEffect
    {//手卡中每有一张矮人单位卡便获得1点强化
        public DwarfMiner (GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var count = Game.GetPlaceCards(PlayerIndex)
                .Concat(Game.PlayersHandCard[PlayerIndex])
                .Concat(Game.PlayersDeck[PlayerIndex])
                .Count(x => x.Status.CardId == Card.Status.CardId);
            await Card.Effect.Strengthen(count, Card);
            return 0;
        }
    }
}
