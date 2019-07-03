namespace Cynthia.Card
{
    //增加护甲前
    public class AfterCardAddArmor : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }

        public AfterCardAddArmor(GameCard target, int num, GameCard source)
        {
            Target = target;
            Num = num;
            Source = source;
        }
    }
}