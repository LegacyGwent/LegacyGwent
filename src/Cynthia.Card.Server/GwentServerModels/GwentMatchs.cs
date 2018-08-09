using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentMatchs
    {
        public IList<GwentRoom> GwentRooms { get; set; } = new List<GwentRoom>();
        private IHubContext<GwentHub> _hub;
        public GwentMatchs(Func<IHubContext<GwentHub>> hub)
        {
            _hub = hub();
        }
        public async void StartGame(GwentRoom room)
        {
            //通知玩家游戏开始
            await _hub.Clients.Client(room.Player1.CurrentUser.ConnectionId).SendAsync("MatchResult", true);
            await _hub.Clients.Client(room.Player2.CurrentUser.ConnectionId).SendAsync("MatchResult", true);
            //初始化房间
            var player1 = room.Player1;
            var player2 = room.Player2;
            var gwentGame = new GwentServerGame(player1, player2);
            //开始游戏改变玩家状态
            player1.CurrentUser.UserState = UserState.Play;
            player2.CurrentUser.UserState = UserState.Play;
            //开启游戏
            await gwentGame.Play();
            //结束游戏恢复玩家状态
            player1.CurrentUser.UserState = UserState.Standby;
            player2.CurrentUser.UserState = UserState.Standby;
            //删除玩家
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
        public async Task<bool> StopMatch(string ConnectionId)
        {   //停止匹配,如果玩家没有正在匹配,返回false
            foreach (var room in GwentRooms)
            {
                if (!room.IsReady && room.Player1.CurrentUser.ConnectionId == ConnectionId)
                {
                    room.Player1.CurrentUser.UserState = UserState.Standby;
                    GwentRooms.Remove(room);
                    await _hub.Clients.Client(room.Player1.CurrentUser.ConnectionId).SendAsync("MatchResult", false);
                    room.Player1.CurrentUser.CurrentPlayer = null;
                    return true;
                }
                if (!room.IsReady && room.Player2.CurrentUser.ConnectionId == ConnectionId)
                {
                    room.Player2.CurrentUser.UserState = UserState.Standby;
                    GwentRooms.Remove(room);
                    await _hub.Clients.Client(room.Player2.CurrentUser.ConnectionId).SendAsync("MatchResult", false);
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