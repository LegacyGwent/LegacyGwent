namespace Cynthia.Card
{
    //卡牌被锁定之后
    public class AfterCardLock : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardLock(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}