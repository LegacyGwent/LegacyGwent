namespace Cynthia.Card
{
    public class GameCard : IHasEffects
    {
        public GameCard()
        {
            Effects = new EffectSet(this);
        }

        //通过卡牌状态,效果,所在半场来创建卡牌
        public GameCard(int playerIndex, CardStatus cardStatus, CardEffect cardEffect) : this()
        {
            PlayerIndex = playerIndex;
            Effect = cardEffect;
            Status = cardStatus;
        }

        //卡牌存在半场
        public int PlayerIndex { get; set; }

        //准备被替换的旧卡牌效果实现
        public CardEffect Effect { get; set; }

        //卡牌的各种信息
        public CardStatus Status { get; set; }

        //卡牌效果合集
        public EffectSet Effects { get; }
    }
}