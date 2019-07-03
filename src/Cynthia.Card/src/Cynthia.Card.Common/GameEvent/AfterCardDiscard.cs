namespace Cynthia.Card
{
    //卡牌被丢弃
    public class AfterCardDiscard : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardDiscard(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}