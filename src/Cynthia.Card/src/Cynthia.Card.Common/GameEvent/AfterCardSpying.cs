namespace Cynthia.Card
{
    //卡牌被标记为间谍后
    public class AfterCardSpying : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardSpying(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}