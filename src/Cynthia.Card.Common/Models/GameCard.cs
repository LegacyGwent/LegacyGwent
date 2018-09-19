namespace Cynthia.Card
{
    public class GameCard
    {
        public GameCard() { }
        public GameCard(int playerIndex, CardStatus cardStatus, CardEffect cardEffect)
        {
            PlayerIndex = playerIndex;
            CardEffect = cardEffect;
            CardStatus = cardStatus;
        }
        //卡牌基本信息索引
        public int PlayerIndex { get; set; }//我方玩家Id
        public CardEffect CardEffect { get; set; }//卡牌效果
        public CardStatus CardStatus { get; set; }
    }
}