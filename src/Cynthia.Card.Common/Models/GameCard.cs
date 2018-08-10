namespace Cynthia.Card
{
    public class GameCard
    {
        public GameCard(string cardIndex) => CardIndex = cardIndex;
        public GameCard() => Conceal = true;
        //卡牌基本信息索引
        public string CardIndex { get; private set; }
        public GwentCard CardInfo
        {
            get
            {
                if (_cardInfo == null)
                    _cardInfo = GwentMap.CardMap[CardIndex];
                return _cardInfo;
            }
        }
        private GwentCard _cardInfo;
        //---------------------------------------
        //卡牌状态
        public bool Visible { get; set; } = false;//是否被揭示 | 手牌
        public bool IsShield { get; set; } = false;//是否昆恩 | 手牌,场地
        public int Armor { get; set; } = 0;//护甲 | 场地
        public int Strength { get; set; }//战力 | 手牌,场地,墓地
        public int HealthStatus { get; set; } = 0;//增益减益 | 手牌,场地
        public bool IsLock { get; set; } = false;//是否锁定 | 场地,墓地
        public bool Conceal { get; set; } = false;//是否盖牌 | 场地
    }
}