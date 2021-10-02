using System.Collections.Generic;

namespace Cynthia.Card.Server
{
    public class User
    {
        public string UserName { get; set; }
        public string PlayerName { get; set; }//玩家名
        public string ConnectionId { get; set; }//链接ID
        public IList<DeckModel> Decks { get; set; }
        public BlacklistModel Blacklist { get; set; }
        public ClientPlayer CurrentPlayer { get; set; }
        public UserState UserState { get; set; }
        public User(string userName, string connectionId, UserState userState = UserState.Standby)
        {
            UserName = userName;
            ConnectionId = connectionId;
            UserState = userState;
        }
    }
}