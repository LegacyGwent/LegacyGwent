namespace Cynthia.Card
{
    //进行了一次交换之后
    public class AfterDrawSwapCard : Event
    {
        public GameCard HandCard { get; set; }

        public AfterDrawSwapCard(GameCard handCard)
        {
            HandCard = handCard;
        }
    }
}