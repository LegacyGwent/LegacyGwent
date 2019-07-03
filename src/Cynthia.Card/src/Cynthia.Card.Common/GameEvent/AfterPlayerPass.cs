namespace Cynthia.Card
{
    //玩家Pass后
    public class AfterPlayerPass : Event
    {
        public int PlayerIndex { get; set; }

        public AfterPlayerPass(int playerIndex)
        {
            PlayerIndex = playerIndex;
        }
    }
}