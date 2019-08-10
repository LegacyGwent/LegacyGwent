using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("44028")]//蓝衣铁卫斥候
    public class BlueStripesScout : CardEffect
    {//使己方半场其他“泰莫利亚”单位，以及手牌和牌组所有战力与自身相同的非间谍“泰莫利亚”单位获得1点增益。 操控。
        public BlueStripesScout(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.GetPlaceCards(PlayerIndex).Where(x => x.HasAllCategorie(Categorie.Temeria)).Concat(Game.PlayersHandCard[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow)).Concat(Game.PlayersDeck[PlayerIndex].Where(x => x.CardInfo().CardUseInfo == CardUseInfo.MyRow)).FilterCards(filter: x => x.ToHealth().health == Card.ToHealth().health);
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