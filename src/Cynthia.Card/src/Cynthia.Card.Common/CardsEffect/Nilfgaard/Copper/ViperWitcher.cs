using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34022")]//毒蛇学派猎魔人
    public class ViperWitcher : CardEffect
    {//己方起始牌组中每有1张“炼金”牌，便造成1点伤害。

        public ViperWitcher(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var point = Card.GetMyBaseDeck(x => x.Categories.Contains(Categorie.Alchemy)).Count;
            var result = await Game.GetSelectPlaceCards(Card);
            if (result.Count <= 0) return 0;
            await result.Single().Effect.Damage(point, Card);
            return 0;
        }
    }
}