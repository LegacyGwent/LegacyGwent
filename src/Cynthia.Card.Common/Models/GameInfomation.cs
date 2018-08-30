using System.Collections.Generic;

namespace Cynthia.Card
{
    public class GameInfomation
    {
        public int MyRow1Point { get; set; }
        public int MyRow2Point { get; set; }
        public int MyRow3Point { get; set; }
        public int EnemyRow1Point { get; set; }
        public int EnemyRow2Point { get; set; }
        public int EnemyRow3Point { get; set; }
        public bool IsMyPlayersPass { get; set; }//我方pass
        public bool IsEnemyPlayersPass { get; set; }//对手pass
        public bool IsMyLeader { get; set; }//我方领袖是否使用
        public bool IsEnemyLeader { get; set; }//敌方领袖是否使用
        public GameCard MyLeader { get; set; }//我方领袖是?
        public GameCard EnemyLeader { get; set; }//敌方领袖是?
        public string EnemyName { get; set; }//对手名称
        public string MyName { get; set; }//对手名称
        public int MyHandCount { get; set; }//我方手牌数量
        public int EnemyHandCount { get; set; }//敌方手牌数量
        public int MyCemeteryCount { get; set; }//我方墓地数量
        public int EnemyCemeteryCount { get; set; }//敌方墓地数量
        public int EnemyDeckCount { get; set; }//对手剩余卡组数量
        public int MyDeckCount { get; set; }//我方剩余卡组数量
        public int MyWinCount { get; set; }//我方剩余卡组数量
        public int EnemyWinCount { get; set; }//我方剩余卡组数量
        public IEnumerable<GameCard> MyHandCard { get; set; }//我方手牌(数量)
        public IEnumerable<GameCard> EnemyHandCard { get; set; }//敌方手牌(数量)
        public IEnumerable<GameCard>[] MyPlace { get; set; }//我方场地
        public IEnumerable<GameCard>[] EnemyPlace { get; set; }//敌方场地
        public IEnumerable<GameCard> MyCemetery { get; set; }//我方墓地
        public IEnumerable<GameCard> EnemyCemetery { get; set; }//敌方墓地
    }
}