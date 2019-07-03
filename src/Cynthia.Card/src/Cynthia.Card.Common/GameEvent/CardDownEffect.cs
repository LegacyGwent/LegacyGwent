namespace Cynthia.Card
{
    public class CardDownEffect : Event
    {

        public bool IsSpying { get; set; }

        public bool IsReveal { get; set; }

        public CardDownEffect(bool isSpying, bool isReveal)
        {
            IsSpying = isSpying;
            IsReveal = isReveal;
        }
    }
}