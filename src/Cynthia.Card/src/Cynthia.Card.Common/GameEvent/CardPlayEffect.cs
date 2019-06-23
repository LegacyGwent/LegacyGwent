namespace Cynthia.Card
{
    public class CardPlayEffect : Event
    {
        public bool IsSpying { get; set; }

        //检索数量
        public int SearchCount { get; set; } = 0;

        public CardPlayEffect(bool isSpying)
        {
            IsSpying = isSpying;
        }
    }
}