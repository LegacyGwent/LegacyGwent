using System.Collections.Generic;

namespace Cynthia.Card
{
    public class GameInfomation
    {
        public string OpponentName { get; set; }
        public int OpponentCardCount { get; set; }
        public IEnumerable<string> YourHandCard { get; set; }
    }
}