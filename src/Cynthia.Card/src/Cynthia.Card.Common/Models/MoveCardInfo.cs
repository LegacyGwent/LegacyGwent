namespace Cynthia.Card
{
    //卡牌移动
    public class MoveCardInfo
    {
        //原本卡牌所在处
        public CardLocation Source;
        //需要将卡牌移动到的地方
        public CardLocation Target;
        //移动的卡牌...如果有的话
        public CardStatus Card;
    }
}