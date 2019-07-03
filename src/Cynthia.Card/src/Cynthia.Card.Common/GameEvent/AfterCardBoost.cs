namespace Cynthia.Card
{
    //发生增益后
    public class AfterCardBoost : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }

        public AfterCardBoost(GameCard target, int num, GameCard source)
        {
            Target = target;
            Num = num;
            Source = source;
        }
    }
}