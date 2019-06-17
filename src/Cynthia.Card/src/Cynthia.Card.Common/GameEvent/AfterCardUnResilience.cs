namespace Cynthia.Card
{
    //坚韧效果被解除后
    public class AfterCardUnResilience : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardUnResilience(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}