namespace Cynthia.Card.Server
{
    public class UserInfo
    {
        public string PlayerName { get; set; }//玩家名
        public string ConnectionId { get; set; }//链接ID
        public GwentClientPlayer CurrentPlayer { get; set; }
        public UserState UserState;
        public UserInfo(string playerName, string connectionId, UserState userState = UserState.Standby)
        {
            PlayerName = playerName;
            ConnectionId = connectionId;
            UserState = userState;
        }
    }
}