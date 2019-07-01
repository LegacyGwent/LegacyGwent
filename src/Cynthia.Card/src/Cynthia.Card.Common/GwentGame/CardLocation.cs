namespace Cynthia.Card
{
    public class CardLocation
    {
        public CardLocation() { }
        public CardLocation(RowPosition row, int index)
        {
            RowPosition = row;
            CardIndex = index;
        }
        public RowPosition RowPosition { get; set; }
        public int CardIndex { get; set; }

        public static CardLocation operator +(CardLocation cardLocation, int offset)
        {
            cardLocation.CardIndex += offset;
            return cardLocation;
        }

        public static CardLocation operator -(CardLocation cardLocation, int offset)
        {
            cardLocation.CardIndex -= offset;
            return cardLocation;
        }
    }
}