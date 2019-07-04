namespace Cynthia.Card
{
    //进行了一次交换之后
    public class AfterCardSwap : Event
    {
        public GameCard HandCard { get; set; }

        public AfterCardSwap(GameCard handCard)
        {
            HandCard = handCard;
        }
    }
}