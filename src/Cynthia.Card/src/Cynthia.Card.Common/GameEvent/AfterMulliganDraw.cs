namespace Cynthia.Card
{
    //调度被抽到后
    public class AfterMulliganDraw : Event
    {
        public GameCard Target { get; set; }

        public AfterMulliganDraw(GameCard target)
        {
            Target = target;
        }
    }
}