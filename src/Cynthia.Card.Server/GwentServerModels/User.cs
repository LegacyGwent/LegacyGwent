namespace Cynthia.Card.Server
{
    public class User
    {
        public string PlayerName { get; set; }//玩家名
        public string ConnectionId { get; set; }//链接ID
        public ClientPlayer CurrentPlayer { get; set; }
        public UserState UserState;
        public User(string playerName, string connectionId, UserState userState = UserState.Standby)
        {
            PlayerName = playerName;
            ConnectionId = connectionId;
            UserState = userState;
        }
    }
}