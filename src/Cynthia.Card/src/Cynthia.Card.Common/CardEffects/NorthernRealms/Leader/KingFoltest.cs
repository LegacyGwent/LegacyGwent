using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("41003")]//弗尔泰斯特国王
    public class KingFoltest : CardEffect
    {//使己方半场其他单位，以及手牌和牌组中的非间谍单位获得1点增益。 操控。
        public KingFoltest(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.GetPlaceCards(PlayerIndex).Concat(Game.PlayersHandCard[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow)).Concat(Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow)).FilterCards(filter: x => x != Card).ToList();
            if (cards.Count() == 0)
            {
                return 0;
            }
            foreach (var card in cards)
            {
                await card.Effect.Boost(1, Card);
            }
            return 0;
        }
    }
}