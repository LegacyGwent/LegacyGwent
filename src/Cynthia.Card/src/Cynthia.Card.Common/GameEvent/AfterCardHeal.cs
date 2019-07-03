namespace Cynthia.Card
{
    //发生治愈后
    public class AfterCardHeal : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardHeal(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}