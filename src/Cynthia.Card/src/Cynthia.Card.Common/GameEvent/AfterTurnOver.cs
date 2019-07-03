namespace Cynthia.Card
{
    //回合结束后
    public class AfterTurnOver : Event
    {
        public int PlayerIndex { get; set; }

        public AfterTurnOver(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}