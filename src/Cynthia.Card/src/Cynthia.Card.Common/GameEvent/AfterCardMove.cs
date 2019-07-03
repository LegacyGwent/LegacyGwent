namespace Cynthia.Card
{
    //卡牌移动后
    public class AfterCardMove : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardMove(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}