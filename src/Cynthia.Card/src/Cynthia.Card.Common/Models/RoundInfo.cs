namespace Cynthia.Card
{
    //回合开始
    public class RoundInfo
    {
        public bool IsPass { get; set; }
        public int HandCardIndex { get; set; }
        public CardLocation CardLocation { get; set; }
    }
}