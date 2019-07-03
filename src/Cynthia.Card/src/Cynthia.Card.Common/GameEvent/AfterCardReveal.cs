namespace Cynthia.Card
{
    //卡牌被揭示后
    public class AfterCardReveal : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardReveal(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}