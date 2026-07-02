namespace Cynthia.Card
{
    public class AfterRoundPlay : Event
    {
        public int PlayerIndex { get; set; }

        public AfterRoundPlay(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}