namespace Cynthia.Card
{
    //发生强化后
    public class AfterCardStrengthen : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }

        public AfterCardStrengthen(GameCard target, int num, GameCard source)
        {
            Target = target;
            Num = num;
            Source = source;
        }
    }
}