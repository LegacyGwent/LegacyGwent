namespace Cynthia.Card.Server
{
    public class UserInfo
    {
        public string PlayerName { get; set; }//玩家名
        public string ConnectionId { get; set; }//链接ID
        public bool IsPlay { get; set; } = false;
        public UserInfo(string playerName, string connectionId, bool isPlay = false)
        {
            PlayerName = playerName;
            ConnectionId = connectionId;
            IsPlay = isPlay;
        }
    }
}