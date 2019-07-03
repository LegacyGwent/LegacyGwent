namespace Cynthia.Card
{
    //进行了一次交换之后
    public class AfterCardSwap : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardSwap(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}