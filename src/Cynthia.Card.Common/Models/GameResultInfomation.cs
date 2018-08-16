namespace Cynthia.Card
{
    public class GameResultInfomation
    {
        public enum GameStatus
        {
            Win,
            Lose,
            Draw
        }
        public string MyName { get; private set; }
        public string EnemyName { get; private set; }
        public GameStatus GameStatu { get; private set; }
        public int RoundCount { get; private set; }
        public int MyR1Point { get; private set; }
        public int MyR2Point { get; private set; }
        public int MyR3Point { get; private set; }
        public int EnemyR1Point { get; private set; }
        public int EnemyR2Point { get; private set; }
        public int EnemyR3Point { get; private set; }

        public GameResultInfomation(string myName, string enemyName,
            GameStatus gameStatu, int roundCount = 0, int myR1Point = 0, int enemyR1Point = 0,
            int myR2Point = 0, int enemyR2Point = 0, int myR3Point = 0, int enemyR3Point = 0)
        {
            MyName = myName;
            EnemyName = enemyName;
            GameStatu = gameStatu;
            RoundCount = roundCount;
            MyR1Point = myR1Point;
            EnemyR1Point = enemyR1Point;
            MyR2Point = myR2Point;
            EnemyR2Point = enemyR2Point;
            MyR3Point = myR3Point;
            EnemyR3Point = enemyR3Point;
        }
    }
}