namespace Cynthia.Card
{
    //卡牌解除间谍状态后
    public class AfterCardUnSpying : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardUnSpying(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}