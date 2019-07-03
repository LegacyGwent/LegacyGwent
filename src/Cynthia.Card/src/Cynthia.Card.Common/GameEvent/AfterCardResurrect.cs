namespace Cynthia.Card
{
    //卡牌复活后
    public class AfterCardResurrect : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardResurrect(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}