using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Cynthia.Card.Server
{
    public class GwentHub : Hub
    {
        public GwentServerService GwentServerService { get; set; }

        //注册
        public bool Register(string username, string password, string playername) => GwentServerService.Register(username, password, playername);

        //登录
        public async Task<UserInfo> Login(string username, string password) => await GwentServerService.Login(new User(username, Context.ConnectionId), password);

        //上传卡组
        public bool AddDeck(DeckModel deck) => GwentServerService.AddDeck(Context.ConnectionId, deck);

        //删除卡组
        public bool RemoveDeck(string id) => GwentServerService.RemoveDeck(Context.ConnectionId, id);

        //修改卡组
        public bool ModifyDeck(string id, DeckModel deck) => GwentServerService.ModifyDeck(Context.ConnectionId, id, deck);

        //开始匹配
        public bool Match(string deckId) => GwentServerService.Match(Context.ConnectionId, deckId);

        //停止匹配
        public async Task<bool> StopMatch() => await GwentServerService.StopMatch(Context.ConnectionId);

        //游戏内玩家操作
        public Task GameOperation(Operation<UserOperationType> operation) => GwentServerService.GameOperation(operation, Context.ConnectionId);

        //重新连接
        public async Task<bool> Reconnect(string username, string password)=> await GwentServerService.Reconnect(Context.ConnectionId,username,password);
        
        //连接中断
        public override Task OnDisconnectedAsync(Exception exception)
        {
            return GwentServerService.Disconnect(Context.ConnectionId,exception,true);
        }
    }
}