namespace Cynthia.Card
{
    //小局结束后
    public class BeforeRoundStart : Event
    {
        public int RoundCount { get; set; }

        public BeforeRoundStart(int roundCount)
        {
            RoundCount = roundCount;
        }
    }
}