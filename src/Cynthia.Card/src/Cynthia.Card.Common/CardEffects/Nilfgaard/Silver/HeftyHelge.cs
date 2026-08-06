using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("33009")]//重弩海尔格
    public class HeftyHelge : CardEffect
    {//对对方半场非同排上的所有敌军单位造成1点伤害。若被揭示，则对所有敌军单位和被揭示的非间谍敌军单位牌造成1点伤害。
        public HeftyHelge(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var cards = Game.GetPlaceCards(AnotherPlayer)
                .Where(x => x.Status.Type == CardType.Unit)
                .ToList();
            if (!isReveal)
            {
                cards = cards.Where(x => x.Status.CardRow != Card.Status.CardRow).ToList();
            }
            else
            {
                cards.AddRange(Game.PlayersHandCard[AnotherPlayer]
                    .Concat(Game.PlayersDeck[AnotherPlayer])
                    .Where(x => x.Status.Type == CardType.Unit &&
                                x.Status.IsReveal &&
                                !x.Status.IsSpying));
                cards = cards.Distinct().ToList();
            }
            foreach (var card in cards)
            {
                await card.Effect.Damage(1, Card, BulletType.FireBall);
            }
            return 0;
        }
    }
}
