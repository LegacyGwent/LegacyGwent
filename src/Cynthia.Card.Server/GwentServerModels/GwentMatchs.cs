using System.Collections.Generic;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentMatchs
    {
        public IList<GwentRoom> GwentRooms { get; set; } = new List<GwentRoom>();
        public async void StartGame(GwentRoom room)
        {
            var player1 = room.Player1;
            var player2 = room.Player2;
            var gwentGame = new GwentServerGame(player1, player2);
            player1.CurrentUser.UserState = UserState.Play;
            player2.CurrentUser.UserState = UserState.Play;
            await gwentGame.Play();
            player1.CurrentUser.UserState = UserState.Standby;
            player2.CurrentUser.UserState = UserState.Standby;
            GwentRooms.Remove(room);
        }
        public void PlayerJoin(ClientPlayer player)
        {
            foreach (var room in GwentRooms)
            {
                if (!room.IsReady)
                {
                    room.AddPlayer(player);
                    if (room.IsReady)
                    {
                        StartGame(room);
                        return;
                    }
                }
            }
            player.CurrentUser.UserState = UserState.Match;
            GwentRooms.Add(new GwentRoom(player));
            return;
        }
        public bool StopMatch(string ConnectionId)
        {   //停止匹配,如果玩家没有正在匹配,返回false
            foreach (var room in GwentRooms)
            {
                if (!room.IsReady && room.Player1.CurrentUser.ConnectionId == ConnectionId)
                {
                    room.Player1.CurrentUser.UserState = UserState.Standby;
                    GwentRooms.Remove(room);
                    room.Player1.CurrentUser.CurrentPlayer = null;
                    return true;
                }
                if (!room.IsReady && room.Player2.CurrentUser.ConnectionId == ConnectionId)
                {
                    room.Player2.CurrentUser.UserState = UserState.Standby;
                    GwentRooms.Remove(room);
                    room.Player2.CurrentUser.CurrentPlayer = null;
                    return true;
                }
            }
            return false;
        }
        public bool PlayerLeave(string ConnectionId)
        {   //对局中离开, 如果玩家没有正在对局,返回false
            foreach (var room in GwentRooms)
            {
                if (room.IsReady && room.Player1.CurrentUser.ConnectionId == ConnectionId)
                {
                    //还有需要补充的代码
                    //宣告比赛结束,获胜方为Player2
                    room.Player2.CurrentUser.UserState = UserState.Standby;
                    room.Player2.CurrentUser.CurrentPlayer = null;
                    room.Player1.CurrentUser.UserState = UserState.Standby;
                    room.Player1.CurrentUser.CurrentPlayer = null;
                    GwentRooms.Remove(room);
                    return true;
                }
                if (room.IsReady && room.Player2.CurrentUser.ConnectionId == ConnectionId)
                {
                    //还有需要补充的代码
                    //宣告比赛结束,获胜方为Player1
                    room.Player2.CurrentUser.UserState = UserState.Standby;
                    room.Player2.CurrentUser.CurrentPlayer = null;
                    room.Player1.CurrentUser.UserState = UserState.Standby;
                    room.Player1.CurrentUser.CurrentPlayer = null;
                    GwentRooms.Remove(room);
                    return true;
                }
            }
            return false;
        }
    }
}