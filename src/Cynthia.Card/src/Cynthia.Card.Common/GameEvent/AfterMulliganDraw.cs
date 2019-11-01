namespace Cynthia.Card
{
    //进行了一次交换之后
    public class AfterMulliganDraw : Event
    {
        public GameCard Target { get; set; }

        public AfterMulliganDraw(GameCard target)
        {
            Target = target;
        }
    }
}