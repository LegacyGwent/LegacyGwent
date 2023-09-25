namespace Cynthia.Card
{
    //调度换掉后
    public class AfterMulliganOut : Event
    {
        public GameCard Target { get; set; }

        public AfterMulliganOut(GameCard target)
        {
            Target = target;
        }
    }
}