namespace Cynthia.Card
{
    //发生吸血后
    public class AfterCardDrain : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }

        public AfterCardDrain(GameCard target, int num, GameCard source)
        {
            Target = target;
            Num = num;
            Source = source;
        }
    }
}