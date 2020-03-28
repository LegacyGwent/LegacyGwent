using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentMatchs
    {
        public IList<GwentRoom> GwentRooms { get; set; } = new List<GwentRoom>();
        private Func<IHubContext<GwentHub>> _hub;
        private GwentCardDataService _gwentCardTypeServic;
        private GwentServerService _gwentService;
        public GwentMatchs(Func<IHubContext<GwentHub>> hub, GwentCardDataService gwentCardTypeService, GwentServerService gwentService)
        {
            _hub = hub;
            _gwentCardTypeServic = gwentCardTypeService;
            _gwentService = gwentService;
        }
        public async void StartGame(GwentRoom room)
        {
            //通知玩家游戏开始
            await _hub().Clients.Client(room.Player1.CurrentUser.ConnectionId).SendAsync("MatchResult", true);
            await _hub().Clients.Client(room.Player2.CurrentUser.ConnectionId).SendAsync("MatchResult", true);
            //初始化房间
            var player1 = room.Player1;
            var player2 = room.Player2;
            var gwentGame = new GwentServerGame(player1, player2, _gwentCardTypeServic, _gwentService.InvokeGameOver);
            //开始游戏改变玩家状态
            player1.CurrentUser.UserState = UserState.Play;
            player2.CurrentUser.UserState = UserState.Play;
            //开启游戏
            room.CurrentGame = gwentGame;
            await gwentGame.Play();
            GameEnd(room);
        }

        //以密码的方式进行匹配
        public void PlayerJoin(ClientPlayer player, string password)
        {
            foreach (var room in GwentRooms)
            {
                //如果这个房间正在等待玩家加入,并且密匙成功配对
                if (!room.IsReady && room.Password == password)
                {
                    room.AddPlayer(player);
                    if (room.IsReady)
                    {
                        StartGame(room);
                        return;
                    }
                }
            }
            if (password == string.Empty)
            {
                //普通匹配(其实是以空白为密匙进行匹配)
                player.CurrentUser.UserState = UserState.Match;
            }
            else
            {
                //以密匙模式进行匹配
                player.CurrentUser.UserState = UserState.PasswordMatch;
            }
            GwentRooms.Add(new GwentRoom(player, password));
            return;
        }
        public async Task<bool> StopMatch(string ConnectionId)
        {   //停止匹配,如果玩家没有正在匹配,返回false
            foreach (var room in GwentRooms)
            {
                //遍历所有的房间
                //上下两个if效果等同,判断在未准备的房间中是否存在取消准备的玩家,如果有
                if (!room.IsReady && room.Player1 != null && room.Player1.CurrentUser.ConnectionId == ConnectionId)
                {
                    //将这个玩家的状态设置为 "闲置"
                    room.Player1.CurrentUser.UserState = UserState.Standby;
                    //删除掉这个房间
                    GwentRooms.Remove(room);
                    //发送匹配结果,false
                    await _hub().Clients.Client(room.Player1.CurrentUser.ConnectionId).SendAsync("MatchResult", false);
                    //将用户中的"当前玩家"设置为空
                    room.Player1.CurrentUser.CurrentPlayer = null;
                    //成功停止了匹配所以返回true
                    return true;
                }
                else if (!room.IsReady && room.Player2 != null && room.Player2.CurrentUser.ConnectionId == ConnectionId)
                {
                    room.Player2.CurrentUser.UserState = UserState.Standby;
                    GwentRooms.Remove(room);
                    await _hub().Clients.Client(room.Player2.CurrentUser.ConnectionId).SendAsync("MatchResult", false);
                    room.Player2.CurrentUser.CurrentPlayer = null;
                    return true;
                }
            }
            //没能停止匹配返回false
            return false;
        }
        public void GameEnd(GwentRoom room)
        {
            //结束游戏恢复玩家状态
            room.Player1.CurrentUser.UserState = UserState.Standby;
            room.Player2.CurrentUser.UserState = UserState.Standby;
            room.Player1.CurrentUser.CurrentPlayer = null;
            room.Player2.CurrentUser.CurrentPlayer = null;
            //删除房间
            GwentRooms.Remove(room);
        }
        public bool PlayerLeave(string connectionId, Exception exception = null)
        {   //对局中离开, 如果玩家没有正在对局,返回false
            foreach (var room in GwentRooms)
            {
                if (room.IsReady && room.Player1.CurrentUser.ConnectionId == connectionId)
                {
                    //强制结束游戏,将获胜方设定为玩家2(待补充)
                    _ = room.CurrentGame.GameEnd(room.CurrentGame.Player2Index, exception);
                    return true;
                }
                if (room.IsReady && room.Player2.CurrentUser.ConnectionId == connectionId)
                {
                    //强制结束游戏,将获胜方设定为玩家2(待补充)
                    _ = room.CurrentGame.GameEnd(room.CurrentGame.Player1Index, exception);
                    return true;
                }
            }
            return false;
        }
        // public async Task<bool> WaitReconnect(string connectionId, Func<Task<bool>> waitReconnect)
        // {
        //     foreach (var room in GwentRooms)
        //     {
        //         if (room.IsReady && room.Player1.CurrentUser.ConnectionId == connectionId)
        //         {
        //             //强制结束游戏,将获胜方设定为玩家2(待补充)
        //             return await room.CurrentGame.WaitReconnect(room.CurrentGame.Player2Index, waitReconnect);
        //         }
        //         if (room.IsReady && room.Player2.CurrentUser.ConnectionId == connectionId)
        //         {
        //             //强制结束游戏,将获胜方设定为玩家2(待补充)
        //             return await room.CurrentGame.WaitReconnect(room.CurrentGame.Player1Index, waitReconnect);
        //         }
        //     }
        //     return false;
        // }
    }
}