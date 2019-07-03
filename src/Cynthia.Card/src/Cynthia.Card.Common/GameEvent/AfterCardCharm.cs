namespace Cynthia.Card
{
    //卡牌被魅惑后
    public class AfterCardCharm : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardCharm(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}