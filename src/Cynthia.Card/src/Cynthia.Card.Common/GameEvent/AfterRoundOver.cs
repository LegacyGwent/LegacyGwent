namespace Cynthia.Card
{
    //小局结束后
    public class AfterRoundOver : Event
    {
        public int RoundCount { get; set; }

        public int Player1Point { get; set; }

        public int Player2Point { get; set; }

        public AfterRoundOver(int roundCount, int player1Point, int player2Point)
        {
            RoundCount = roundCount;
            Player1Point = player1Point;
            Player2Point = player2Point;
        }
    }
}