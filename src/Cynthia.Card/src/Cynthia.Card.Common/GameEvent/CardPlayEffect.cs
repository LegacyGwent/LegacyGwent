namespace Cynthia.Card
{
    public class CardPlayEffect : Event
    {
        public bool IsSpying { get; set; }

        public bool IsReveal { get; set; }

        //检索数量
        public int SearchCount { get; set; } = 0;

        public CardPlayEffect(bool isSpying, bool isReveal)
        {
            IsSpying = isSpying;
            IsReveal = isReveal;
        }
    }
}