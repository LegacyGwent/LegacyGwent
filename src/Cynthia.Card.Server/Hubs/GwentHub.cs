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
        public UserInfo Login(string username, string password) => GwentServerService.Login(new User(username, Context.ConnectionId), password);
        //上传卡组
        public bool AddDeck(DeckModel deck) => GwentServerService.AddDeck(Context.ConnectionId, deck);
        //删除卡组
        public bool RemoveDeck(int deckIndex) => GwentServerService.RemoveDeck(Context.ConnectionId, deckIndex);
        //修改卡组
        public bool ModifyDeck(int deckIndex, DeckModel deck) => GwentServerService.ModifyDeck(Context.ConnectionId, deckIndex, deck);
        //开始匹配
        public bool Match(int cardIndex) => GwentServerService.Match(Context.ConnectionId, cardIndex);
        //停止匹配
        public async Task<bool> StopMatch() => await GwentServerService.StopMatch(Context.ConnectionId);
        //游戏内玩家操作
        public Task GameOperation(Operation<UserOperationType> operation) => GwentServerService.GameOperation(operation, Context.ConnectionId);
        //连接中断
        public override Task OnDisconnectedAsync(Exception exception)
        {
            GwentServerService.Disconnect(Context.ConnectionId);
            return Task.CompletedTask;
        }
    }
}