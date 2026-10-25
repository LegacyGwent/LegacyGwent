using System.Linq;

namespace Cynthia.Card
{
    // A source-attributed unit destruction, including Doomed deaths but not active banish.
    // Capture the source before queued cemetery repair/transform changes its state.
    public sealed class AfterCardDestroyed : Event
    {
        public GameCard Target { get; }
        public GameCard Source { get; }
        public int SourcePlayerIndex { get; }
        public CardType SourceType { get; }
        public Categorie[] SourceCategories { get; }

        public AfterCardDestroyed(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
            SourcePlayerIndex = source?.PlayerIndex ?? -1;
            SourceType = source?.Status.Type ?? CardType.Special;
            SourceCategories = source?.Status.Categories.ToArray() ?? new Categorie[0];
        }
    }
}
