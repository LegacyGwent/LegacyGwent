using System.Collections.Generic;

namespace Cynthia.Card
{
    public class NewlyUnlockedTrinkets: ModelBase
    {
        public List<string> NewAvatars { get; set; } = new List<string>();
        public List<string> NewBorders { get; set; } = new List<string>();
        public List<string> NewTitles { get; set; } = new List<string>();

        public bool HasNewTrinkets => NewAvatars.Count > 0 || NewBorders.Count > 0 || NewTitles.Count > 0;

        public void Clear()
        {
            NewAvatars.Clear();
            NewBorders.Clear();
            NewTitles.Clear();
        }
    }
}
