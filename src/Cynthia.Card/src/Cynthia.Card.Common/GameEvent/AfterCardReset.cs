namespace Cynthia.Card
{
    //卡牌被重置后
    public class AfterCardReset : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardReset(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}