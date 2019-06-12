using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34019")]//火蝎攻城弩
    public class FireScorpion : CardEffect
    {//对1个敌军单位造成5点伤害。被己方揭示时，再次触发此能力。
        public FireScorpion(IGwentServerGame game, GameCard card) : base(game, card) { }
        public override async Task<int> CardPlayEffect(bool isSpying)
        {
            var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
            if (result.Count != 0) await result.Single().Effect.Damage(5, Card);
            return 0;
        }

        public override async Task OnCardReveal(GameCard target, GameCard soure = null)
        {
            if (target != Card || soure == null || soure.PlayerIndex != Card.PlayerIndex) return;
            var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
            if (result.Count != 0) await result.Single().Effect.Damage(5, Card);
        }
    }
}