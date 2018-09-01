namespace Cynthia.Card
{
    //回合开始
    public class RoundInfo
    {
        public bool IsPass { get; set; }
        public int HandCardIndex { get; set; }
        public int RowIndex { get; set; }
        public int CardIndex { get; set; }
    }
}