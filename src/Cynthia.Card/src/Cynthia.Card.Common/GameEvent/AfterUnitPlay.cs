namespace Cynthia.Card
{
    //单位卡打出后
    public class AfterUnitPlay : Event
    {
        public GameCard PlayedCard { get; set; }

        public bool IsFromHand { get; set; }

        public bool IsSpying { get; set; }

        public bool IsReveal { get; set; }

        public bool IsFromAnother { get => !IsFromHand; }

        public AfterUnitPlay(GameCard playedCard, bool isFromHand, bool isSpying, bool isReveal)
        {
            IsSpying = isSpying;
            PlayedCard = playedCard;
            IsFromHand = isFromHand;
            IsReveal = isReveal;
        }
    }
}