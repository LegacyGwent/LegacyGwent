using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("34019")]//火蝎攻城弩
    public class FireScorpion : CardEffect, IHandlesEvent<AfterCardReveal>
    {//对1个敌军单位造成5点伤害。被己方揭示时，再次触发此能力。
        public FireScorpion(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
            if (result.Count != 0) await result.Single().Effect.Damage(5, Card);
            return 0;
        }

        public async Task HandleEvent(AfterCardReveal @event)
        {
            if (@event.Target != Card || @event.Source == null || @event.Source.PlayerIndex != Card.PlayerIndex) return;
            var result = (await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow));
            if (result.Count != 0) await result.Single().Effect.Damage(5, Card);
        }
    }
}