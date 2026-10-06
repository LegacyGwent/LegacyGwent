using System.Collections.Generic;

namespace Cynthia.Card
{
    public class UserInfo : ModelBase
    {
        public object this[string propertyName] // allows the user["property"] syntax
        {
            get { return this.GetType().GetProperty(propertyName).GetValue(this, null); }
            set { this.GetType().GetProperty(propertyName).SetValue(this, value, null); }
        }
        public string PlayerName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public IList<string> UserMessages { get; set; } = new List<string>();
        public IList<DeckModel> Decks { get; set; }
        public BlacklistModel Blacklist { get; set; }
        public int MMR { get; set; }//玩家天梯分数
        public IList<int[]> Streak { get; set; } = new List<int[]> { new int[3], new int[3], new int[3], new int[3], new int[3] };
        public int HighestMMR { get; set; }
        private IList<string> _ownedAvatars = new List<string>();
        private IList<string> _ownedBorders = new List<string>();
        private IList<string> _ownedTitles = new List<string>();
        private NewlyUnlockedTrinkets _newlyUnlockedTrinkets = new NewlyUnlockedTrinkets();
        // Missing fields in legacy documents and explicit JSON null have the same contract.
        // Empty ownership does not grant a cosmetic; equipped defaults are assigned at login.
        public IList<string> OwnedAvatars { get => _ownedAvatars; set => _ownedAvatars = value ?? new List<string>(); }
        public IList<string> OwnedBorders { get => _ownedBorders; set => _ownedBorders = value ?? new List<string>(); }
        public IList<string> OwnedTitles { get => _ownedTitles; set => _ownedTitles = value ?? new List<string>(); }
        public string CurrentAvatar { get; set; }
        public string CurrentBorder { get; set; }
        public string CurrentTitle { get; set; }
        public int GGsReceived { get; set; }
        public int GamesOver200 { get; set; }
        public NewlyUnlockedTrinkets NewlyUnlockedTrinkets { get => _newlyUnlockedTrinkets; set => _newlyUnlockedTrinkets = value ?? new NewlyUnlockedTrinkets(); }
    }
}
