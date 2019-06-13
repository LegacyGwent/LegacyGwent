using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("14004")]//蟹蜘蛛毒液
    public class ArachasVenom : CardEffect
    {//对3个相邻单位造成4点伤害。
        public ArachasVenom(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardUseEffect()
        {
            var result = await Game.GetSelectPlaceCards(Card, range: 1);
            if (result.Count <= 0) return 0;
            foreach (var card in result.Single().GetRangeCard(1).ToList())
            {
                if (card.Status.CardRow.IsOnPlace())
                    await card.Effect.Damage(4, Card);
            }
            return 0;
        }
    }
}