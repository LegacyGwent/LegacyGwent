using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("62003")]//维伯约恩
    public class Vabjorn : CardEffect
    {//对1个单位造成2点伤害。若目标已受伤，则将其摧毁。
        public Vabjorn(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            var result = await Game.GetSelectPlaceCards(Card, range: 1);
            if (result.Count <= 0) return 0;
                foreach (var card in result.Single().GetRangeCard(1).ToList())
                    {
                        if (card.Status.HealthStatus >= 0)
                        {
                            await card.Effect.Damage(2, Card);
                        }
                        else if (card.Status.HealthStatus < 0)
                        {
                        await card.Effect.ToCemetery(CardBreakEffectType.Scorch);
                        }
                    }
            return 0;
        }
    }
}