using System.Collections.Generic;

namespace Cynthia.Card
{
    public class UserInfo : ModelBase
    {
        public string PlayerName { get; set; }
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public IList<DeckModel> Decks { get; set; }
        public BlacklistModel Blacklist { get; set; }
    }
}