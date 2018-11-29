namespace Cynthia.Card
{
    public class CardLocation
    {
        public CardLocation(){}
        public CardLocation(RowPosition row,int index)
        {
            RowPosition = row;
            CardIndex = index;
        }
        public RowPosition RowPosition { get; set; }
        public int CardIndex { get; set; }
    }
}