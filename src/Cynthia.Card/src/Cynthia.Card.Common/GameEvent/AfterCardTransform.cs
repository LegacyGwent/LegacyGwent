namespace Cynthia.Card
{
    //after a card transforms
    public class AfterCardTransform : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public string PreviousCardId { get; }
        public CardType? PreviousType { get; }
        public string TransformedCardId { get; }
        public CardType TransformedType { get; }

        public AfterCardTransform(GameCard target, GameCard source, string previousCardId = null, CardType? previousType = null)
        {
            PreviousCardId = previousCardId;
            PreviousType = previousType;
            TransformedCardId = target.Status.CardId;
            TransformedType = target.Status.Type;
            Target = target;
            Source = source;
        }
    }
}