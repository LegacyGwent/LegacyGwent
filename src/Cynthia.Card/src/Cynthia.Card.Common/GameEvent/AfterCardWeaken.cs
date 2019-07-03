namespace Cynthia.Card
{
    //卡牌削弱之后
    public class AfterCardWeaken : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }

        public AfterCardWeaken(GameCard target, int num, GameCard source)
        {
            Target = target;
            Num = num;
            Source = source;
        }
    }
}