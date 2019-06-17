namespace Cynthia.Card
{
    //吞噬发生后
    public class AfterCardConsume : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardConsume(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}