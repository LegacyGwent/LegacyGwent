using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    [CardEffectId("70151")]//菲利普·凡·莫拉汉姆 PhilippevanMoorlehem
    public class PhilippevanMoorlehem : CardEffect, IHandlesEvent<AfterTurnOver>, IHandlesEvent<AfterCardReveal>
    {//
        public PhilippevanMoorlehem(GameCard card) : base(card) { }
        public override async Task<int> CardPlayEffect(bool isSpying,bool isReveal)
        {
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return 0;
            }
            await target.Effect.Damage(3,Card);
            return 0;
        }
        public async Task HandleEvent(AfterTurnOver @event)
        {
            if (@event.PlayerIndex != Card.PlayerIndex || !Card.Status.CardRow.IsInHand())
            {
                return;
            }
            await Card.Effect.Reveal(Card);
        }
        public async Task HandleEvent(AfterCardReveal @event)
        {
            if(@event.Target != Card)
            {
                return;
            }
            var selectList = await Game.GetSelectPlaceCards(Card, selectMode: SelectModeType.EnemyRow);
            if (!selectList.TrySingle(out var target))
            {
                return;
            }
            if (@event.Target != Card || @event.Source == null || @event.Source.PlayerIndex != Card.PlayerIndex) return;
            await target.Effect.Damage(3,Card);
        }
    }
}
