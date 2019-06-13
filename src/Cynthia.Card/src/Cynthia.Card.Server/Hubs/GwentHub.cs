using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentHub : Hub
    {
        public GwentServerService _gwentServerService;

        public GwentHub(GwentServerService gwentServerService) => _gwentServerService = gwentServerService;

        //注册
        public bool Register(string username, string password, string playername) => _gwentServerService.Register(username, password, playername);

        //登录
        public async Task<UserInfo> Login(string username, string password) => await _gwentServerService.Login(new User(username, Context.ConnectionId), password);

        //上传卡组
        public bool AddDeck(DeckModel deck) => _gwentServerService.AddDeck(Context.ConnectionId, deck);

        //删除卡组
        public bool RemoveDeck(string id) => _gwentServerService.RemoveDeck(Context.ConnectionId, id);

        //修改卡组
        public bool ModifyDeck(string id, DeckModel deck) => _gwentServerService.ModifyDeck(Context.ConnectionId, id, deck);

        //开始匹配
        public bool Match(string deckId) => _gwentServerService.Match(Context.ConnectionId, deckId);

        //停止匹配
        public async Task<bool> StopMatch() => await _gwentServerService.StopMatch(Context.ConnectionId);

        //获取在线人数
        public async Task<int> GetUserCount() => await _gwentServerService.GetUserCount(Context.ConnectionId);

        //游戏内玩家操作
        public Task GameOperation(Operation<UserOperationType> operation)
        {
            var result = _gwentServerService.GameOperation(operation, Context.ConnectionId);
            return result;
        }

        //重新连接
        // public async Task<bool> Reconnect(string username, string password) => await _gwentServerService.Reconnect(Context.ConnectionId, username, password);

        //连接中断
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return _gwentServerService.Disconnect(Context.ConnectionId, exception);//, true);
        }
    }
}