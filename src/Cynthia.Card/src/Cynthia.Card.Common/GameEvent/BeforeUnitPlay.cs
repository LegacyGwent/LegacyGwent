namespace Cynthia.Card
{
    //单位卡打出后
    public class BeforeUnitPlay : Event
    {
        public GameCard PlayedCard { get; set; }

        public bool IsFromHand { get; set; }

        public bool IsFromAnother { get => !IsFromHand; }

        public BeforeUnitPlay(GameCard playedCard, bool isFromHand)
        {
            PlayedCard = playedCard;
            IsFromHand = isFromHand;
        }
    }
}