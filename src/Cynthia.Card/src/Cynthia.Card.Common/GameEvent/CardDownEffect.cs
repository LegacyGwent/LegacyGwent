namespace Cynthia.Card
{
    public class CardDownEffect : Event
    {

        public bool IsSpying { get; set; }

        public CardDownEffect(bool isSpying)
        {
            IsSpying = isSpying;
        }
    }
}