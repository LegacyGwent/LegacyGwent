namespace Cynthia.Card
{
    public class GameCard
    {
        public GameCard(string cardIndex) => CardIndex = cardIndex;
        public GameCard() => Conceal = true;
        //卡牌基本信息索引
        public string CardIndex { get; set; }
        public GwentCard CardInfo { get; set; }
        //---------------------------------------
        //卡牌状态
        public bool IsReveal { get; set; } = false;//是否被揭示 | 手牌
        public bool IsShield { get; set; } = false;//是否昆恩 | 手牌,场地
        public bool IsSpying { get; set; } = false;//是否间谍 | 场地
        public bool IsResilience { get; set; } = false;//是否坚韧 | 场地
        public int Armor { get; set; } = 0;//护甲 | 场地
        public int Strength { get; set; }//战力 | 手牌,场地,墓地
        public int HealthStatus { get; set; } = 0;//增益减益 | 手牌,场地
        public bool IsLock { get; set; } = false;//是否锁定 | 场地,墓地
        public bool Conceal { get; set; } = false;//是否盖牌 | 场地
        //------------------------------------------
        //显示相关
        public bool IsGray { get; set; } = true;
        public bool IsCardBack { get; set; } = false;
        public Faction DeckFaction { get; set; } = Faction.Monsters;
    }
}