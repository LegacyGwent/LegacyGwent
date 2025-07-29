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
        public IList<DeckModel> Decks { get; set; }
        public BlacklistModel Blacklist { get; set; }
        public int MMR { get; set; }//玩家天梯分数
        public int HighestMMR { get; set; }
        public IList<string> OwnedAvatars { get; set; }
        public IList<string> OwnedBorders { get; set; }
        public IList<string> OwnedTitles { get; set; }
        public string CurrentAvatar { get; set; }
        public string CurrentBorder { get; set; }
        public string CurrentTitle { get; set; }
        public int GGsReceived { get; set; }
        public int GamesOver200 { get; set; }
    }
}
