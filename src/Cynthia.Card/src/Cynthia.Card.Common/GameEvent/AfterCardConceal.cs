namespace Cynthia.Card
{
    //发生隐匿后
    public class AfterCardConceal : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardConceal(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}