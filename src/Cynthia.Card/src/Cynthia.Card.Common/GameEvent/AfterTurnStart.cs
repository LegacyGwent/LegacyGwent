namespace Cynthia.Card
{
    //回合开始后(时)
    public class AfterTurnStart : Event
    {
        public int PlayerIndex { get; set; }

        public AfterTurnStart(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}