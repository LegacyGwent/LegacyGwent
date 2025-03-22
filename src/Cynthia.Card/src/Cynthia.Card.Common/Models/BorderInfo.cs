using System.Collections.Generic;

namespace Cynthia.Card
{
    public class Border : ModelBase
    {
        public string ID { get; set; }
        public bool IsReleased { get; set; }
        public int UnlockCounter { get; set; }
        public string UnlockStat { get; set; }
    }

}