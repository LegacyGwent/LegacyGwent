using System.Threading.Tasks;

namespace Cynthia.Card
{
    [CardEffectId("52003")]//泽维尔·莫兰
    public class XavierMoran : CardEffect, IHandlesEvent<AfterRoundOver>, IHandlesEvent<AfterUnitDown>
    {
        public XavierMoran(GameCard card) : base(card) { }
        private int _strongestDwarf;

        public override async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Boost(_strongestDwarf, Card);
            return 0;
        }

        // Keep the DIY implementation's landing contract: it observes allied
        // dwarf landings in every zone, snapshots base Strength, and excludes self.
        public Task HandleEvent(AfterUnitDown @event)
        {
            if (@event.Target != Card && @event.Target.PlayerIndex == PlayerIndex &&
                @event.Target.HasAllCategorie(Categorie.Dwarf) &&
                @event.Target.Status.Strength > _strongestDwarf)
                _strongestDwarf = @event.Target.Status.Strength;
            return Task.CompletedTask;
        }

        public Task HandleEvent(AfterRoundOver @event)
        {
            _strongestDwarf = 0;
            return Task.CompletedTask;
        }
    }
}
