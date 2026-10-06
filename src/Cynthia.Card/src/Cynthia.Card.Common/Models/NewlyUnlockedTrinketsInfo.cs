using System.Collections.Generic;

namespace Cynthia.Card
{
    public class NewlyUnlockedTrinkets: ModelBase
    {
        private List<string> _newAvatars = new List<string>();
        private List<string> _newBorders = new List<string>();
        private List<string> _newTitles = new List<string>();
        public List<string> NewAvatars { get => _newAvatars; set => _newAvatars = value ?? new List<string>(); }
        public List<string> NewBorders { get => _newBorders; set => _newBorders = value ?? new List<string>(); }
        public List<string> NewTitles { get => _newTitles; set => _newTitles = value ?? new List<string>(); }

        public bool HasNewTrinkets => NewAvatars.Count > 0 || NewBorders.Count > 0 || NewTitles.Count > 0;

        public void Clear()
        {
            NewAvatars.Clear();
            NewBorders.Clear();
            NewTitles.Clear();
        }
    }
}
