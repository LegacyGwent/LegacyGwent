namespace Cynthia.Card
{
    //卡牌被解锁后
    public class AfterCardUnLock : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardUnLock(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}