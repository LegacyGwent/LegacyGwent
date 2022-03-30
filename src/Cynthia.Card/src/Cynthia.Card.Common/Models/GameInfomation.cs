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
        public bool IsEnemyPlayerMulligan { get; set; }//对方是否调度中
        public bool IsMyPlayerMulligan { get; set; }//我方是否调度中
        public bool IsMyPlayerPass { get; set; }//我方pass
        public bool IsEnemyPlayerPass { get; set; }//对手pass
        public bool IsMyLeader { get; set; }//我方领袖是否使用
        public bool IsEnemyLeader { get; set; }//敌方领袖是否使用
        public CardStatus MyLeader { get; set; }//我方领袖是?
        public CardStatus EnemyLeader { get; set; }//敌方领袖是?
        public string EnemyName { get; set; }//对手名称
        public string MyName { get; set; }//对手名称
        public int MyMMR{get;set;}//我方mmr
        public int EnemyMMR{get;set;}//敌方mmr
        public int MyHandCount { get; set; }//我方手牌数量
        public int EnemyHandCount { get; set; }//敌方手牌数量
        public int MyCemeteryCount { get; set; }//我方墓地数量
        public int EnemyCemeteryCount { get; set; }//敌方墓地数量
        public int EnemyDeckCount { get; set; }//对手剩余卡组数量
        public int MyDeckCount { get; set; }//我方剩余卡组数量
        public int MyWinCount { get; set; }//我方剩余卡组数量
        public int EnemyWinCount { get; set; }//我方剩余卡组数量
        public IEnumerable<CardStatus> MyHandCard { get; set; }//我方手牌(数量)
        public IEnumerable<CardStatus> EnemyHandCard { get; set; }//敌方手牌(数量)
        public IEnumerable<CardStatus> MyStay { get; set; }//我方悬牌
        public IEnumerable<CardStatus> EnemyStay { get; set; }//敌方悬牌
        public IEnumerable<CardStatus>[] MyPlace { get; set; }//我方场地
        public IEnumerable<CardStatus>[] EnemyPlace { get; set; }//敌方场地
        public IEnumerable<CardStatus> MyCemetery { get; set; }//我方墓地
        public IEnumerable<CardStatus> EnemyCemetery { get; set; }//敌方墓地
    }
}