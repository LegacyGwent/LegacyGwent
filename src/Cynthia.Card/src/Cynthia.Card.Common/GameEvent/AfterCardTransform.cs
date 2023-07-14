namespace Cynthia.Card
{
    //after a card transforms
    public class AfterCardTransform : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardTransform(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}