using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Cynthia.Card.AI;

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
        public async void StartGame(GwentRoom room, bool isSpecial = false, bool isCountMMR = false)
        {
            //通知玩家游戏开始
            if (room.Player1 is ClientPlayer)
            {
                await _hub().Clients.Client((room.Player1 as ClientPlayer).CurrentUser.ConnectionId).SendAsync("MatchResult", true);
            }
            if (room.Player2 is ClientPlayer)
            {
                await _hub().Clients.Client((room.Player2 as ClientPlayer).CurrentUser.ConnectionId).SendAsync("MatchResult", true);
            }
            //初始化房间
            var player1 = room.Player1;
            var player2 = room.Player2;
            var gwentGame = new GwentServerGame(player1, player2, _gwentCardTypeServic, result => _gwentService.InvokeGameOver(result, (player1 is AIPlayer || player2 is AIPlayer), isCountMMR), isSpecial);
            //开始游戏改变玩家状态
            if (room.Player1 is ClientPlayer)
            {
                (room.Player1 as ClientPlayer).CurrentUser.UserState = room.Player2 is AIPlayer ? UserState.PlayWithAI : UserState.Play;
            }
            if (room.Player2 is ClientPlayer)
            {
                (room.Player2 as ClientPlayer).CurrentUser.UserState = room.Player1 is AIPlayer ? UserState.PlayWithAI : UserState.Play;
            }
            //开启游戏
            room.CurrentGame = gwentGame;
            await gwentGame.Play();
            GameEnd(room);
            _gwentService.InovkeUserChanged();
        }

        //以密码的方式进行匹配
        public void PlayerJoin(ClientPlayer player, string password)
        {
            //判断是否是特殊密码
            if (password.ToLower().EndsWith("#f"))
            {
                switch (password.ToLower().Replace("#f", "").Replace("special", ""))
                {
                    case "ai":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new GeraltNovaAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "aild":
                    case "ldai":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new GeraltNovaAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai1":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new SoldierTrainAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai1ld":
                    case "ldai1":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new SoldierTrainAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai2":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new MillAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai2ld":
                    case "ldai2":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new MillAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai3":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new AuberonKingAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai3ld":
                    case "ldai3":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new AuberonKingAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai4":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new IronFalconAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai4ld":
                    case "ldai4":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new IronFalconAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai5":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new ReaverHunterAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai5ld":
                    case "ldai5":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new ReaverHunterAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    default:
                        break;
                }
            }

            foreach (var room in GwentRooms)
            {
                //如果这个房间正在等待玩家加入,并且密匙成功配对
                if (!room.IsReady && ((room.Password.ToLower() == password.ToLower() && !room.InBlacklist(player)) || (room.Password == string.Empty && password.ToLower().StartsWith("ai"))))
                {
                    room.AddPlayer(player);
                    if (room.IsReady)
                    {
                        StartGame(room, password.ToLower() == "ld", room.Password == "rank");
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
                switch (password.ToLower().Replace("#f", "").Replace("special", ""))
                {
                    case "ai":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new GeraltNovaAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "aild":
                    case "ldai":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new GeraltNovaAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai1":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new SoldierTrainAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai1ld":
                    case "ldai1":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new SoldierTrainAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai2":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new MillAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai2ld":
                    case "ldai2":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new MillAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai3":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new AuberonKingAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai3ld":
                    case "ldai3":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new AuberonKingAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai4":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new IronFalconAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai4ld":
                    case "ldai4":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new IronFalconAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    case "ai5":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new ReaverHunterAI());
                            GwentRooms.Add(room);
                            StartGame(room);
                            return;
                        }
                    case "ai5ld":
                    case "ldai5":
                        {
                            var room = new GwentRoom(player, password);
                            room.AddPlayer(new ReaverHunterAI());
                            GwentRooms.Add(room);
                            StartGame(room, true);
                            return;
                        }
                    default:
                        break;
                }
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
                if (!room.IsReady && room.Player1 != null && room.Player1 is ClientPlayer && (room.Player1 as ClientPlayer).CurrentUser.ConnectionId == ConnectionId)
                {
                    //将这个玩家的状态设置为 "闲置"
                    (room.Player1 as ClientPlayer).CurrentUser.UserState = UserState.Standby;
                    //删除掉这个房间
                    GwentRooms.Remove(room);
                    //发送匹配结果,false
                    await _hub().Clients.Client((room.Player1 as ClientPlayer).CurrentUser.ConnectionId).SendAsync("MatchResult", false);
                    //将用户中的"当前玩家"设置为空
                    (room.Player1 as ClientPlayer).CurrentUser.CurrentPlayer = null;
                    //成功停止了匹配所以返回true
                    return true;
                }
                else if (!room.IsReady && room.Player2 != null && room.Player2 is ClientPlayer && (room.Player2 as ClientPlayer).CurrentUser.ConnectionId == ConnectionId)
                {
                    (room.Player2 as ClientPlayer).CurrentUser.UserState = UserState.Standby;
                    GwentRooms.Remove(room);
                    await _hub().Clients.Client((room.Player2 as ClientPlayer).CurrentUser.ConnectionId).SendAsync("MatchResult", false);
                    (room.Player2 as ClientPlayer).CurrentUser.CurrentPlayer = null;
                    return true;
                }
            }
            //没能停止匹配返回false
            return false;
        }
        public void GameEnd(GwentRoom room)
        {
            //结束游戏恢复玩家状态
            if (room.Player1 is ClientPlayer)
            {
                (room.Player1 as ClientPlayer).CurrentUser.UserState = UserState.Standby;
                (room.Player1 as ClientPlayer).CurrentUser.CurrentPlayer = null;
            }
            if (room.Player2 is ClientPlayer)
            {
                (room.Player2 as ClientPlayer).CurrentUser.UserState = UserState.Standby;
                (room.Player2 as ClientPlayer).CurrentUser.CurrentPlayer = null;
            }
            //删除房间
            GwentRooms.Remove(room);
        }
        public bool PlayerLeave(string connectionId, Exception exception = null, bool isSurrender = false)
        {   //对局中离开, 如果玩家没有正在对局,返回false
            foreach (var room in GwentRooms)
            {
                if (room.IsReady && room.Player1 is ClientPlayer && (room.Player1 as ClientPlayer).CurrentUser.ConnectionId == connectionId)
                {
                    //强制结束游戏,将获胜方设定为玩家2(待补充)
                    _ = room.CurrentGame.GameEnd(room.CurrentGame.Player2Index, exception, isSurrender);
                    return true;
                }
                if (room.IsReady && room.Player2 is ClientPlayer && (room.Player2 as ClientPlayer).CurrentUser.ConnectionId == connectionId)
                {
                    //强制结束游戏,将获胜方设定为玩家2(待补充)
                    _ = room.CurrentGame.GameEnd(room.CurrentGame.Player1Index, exception, isSurrender);
                    return true;
                }
            }
            return false;
        }
        // Find the running game of a player, with the player's index in that game
        private (GwentServerGame Game, int PlayerIndex)? FindGame(ClientPlayer player)
        {
            foreach (var room in GwentRooms.ToList())
            {
                if (!room.IsReady || room.CurrentGame == null)
                    continue;
                if (room.Player1 == player)
                    return (room.CurrentGame, room.CurrentGame.Player1Index);
                if (room.Player2 == player)
                    return (room.CurrentGame, room.CurrentGame.Player2Index);
            }
            return null;
        }

        public bool IsInGame(ClientPlayer player) => FindGame(player) != null;

        // The player lost the connection; the game waits for a reconnect instead of ending
        public async Task PlayerDisconnected(ClientPlayer player, int waitSeconds)
        {
            var found = FindGame(player);
            if (found == null)
                return;
            var (game, playerIndex) = found.Value;
            var enemyIndex = game.AnotherPlayer(playerIndex);
            // a locale key, translated by the client; shown without an OK button and with a timer bar
            await game.SendMessageBoxAutoClose(enemyIndex, "PopupWindow_OpponentDisconnected", waitSeconds, showTimer: true);
            if (game.Players[enemyIndex] is ClientPlayer enemy)
                await enemy.SendOperactionList();
        }

        // The player is back on a new connection; send the current game state again
        public async Task<bool> PlayerReconnected(ClientPlayer player)
        {
            var found = FindGame(player);
            if (found == null)
                return false;
            var (game, playerIndex) = found.Value;
            await game.ResendGameState(playerIndex);
            var enemyIndex = game.AnotherPlayer(playerIndex);
            await game.SendMessageBoxAutoClose(enemyIndex, "PopupWindow_OpponentReconnected", 3);
            if (game.Players[enemyIndex] is ClientPlayer enemy)
                await enemy.SendOperactionList();
            return true;
        }

        // JoinViewList
        public bool JoinViewList(User user, string roomId)
        {
            foreach (var room in GwentRooms)
            {
                if (room.RoomId == roomId && room.IsReady)
                {
                    return room.CurrentGame.JoinViewList(new Viewer(user, _hub));
                }
            }
            return false;
        }

        public bool LeaveViewList(User user, string roomId)
        {
            if (roomId == "")
            {
                foreach (var room in GwentRooms)
                {
                    if (room.IsReady && room.CurrentGame.ViewList.Any(x => x.CurrentUser.ConnectionId == user.ConnectionId))
                    {
                        return room.CurrentGame.LeaveViewList(user);
                    }
                }
                return true;
            }

            foreach (var room in GwentRooms)
            {
                if (room.RoomId == roomId && room.IsReady)
                {
                    return room.CurrentGame.LeaveViewList(user);
                }
            }
            return true;
        }
    }
}
