namespace Cynthia.Card
{
    //进行了一次交换之后
    public class AfterMulliganOut : Event
    {
        public GameCard Target { get; set; }

        public AfterMulliganOut(GameCard target)
        {
            Target = target;
        }
    }
}