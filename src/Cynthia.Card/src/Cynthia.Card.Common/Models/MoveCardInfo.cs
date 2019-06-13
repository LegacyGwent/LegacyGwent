namespace Cynthia.Card
{
    //卡牌移动
    public class MoveCardInfo
    {
        //原本卡牌所在处
        public CardLocation Soure;
        //需要将卡牌移动到的地方
        public CardLocation Taget;
        //移动的卡牌...如果有的话
        public CardStatus Card;
    }
}