namespace Cynthia.Card
{
    //发生坚韧后
    public class AfterCardResilience : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardResilience(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}