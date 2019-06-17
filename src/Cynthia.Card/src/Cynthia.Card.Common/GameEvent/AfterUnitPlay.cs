namespace Cynthia.Card
{
    //单位卡打出后
    public class AfterUnitPlay : Event
    {
        public GameCard PlayedCard { get; set; }

        public AfterUnitPlay(GameCard playedCard)
        {
            PlayedCard = playedCard;
        }
    }
}