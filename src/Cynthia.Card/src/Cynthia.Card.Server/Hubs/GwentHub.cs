using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;

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

        //上传卡组码
        public bool AddDeckCode(string deckCode) => _gwentServerService.AddDeck(Context.ConnectionId, deckCode.DeCompressToDeck());

        //上传卡组码
        public bool AddDeckCodeWithName(string deckCode, string deckName)
        {
            var deck = deckCode.DeCompressToDeck();
            if (deckName != null)
            {
                deck.Name = deckName;
            }
            return _gwentServerService.AddDeck(Context.ConnectionId, deckCode.DeCompressToDeck());
        }

        //上传卡组
        public bool AddDeck(DeckModel deck) => _gwentServerService.AddDeck(Context.ConnectionId, deck);

        //删除卡组
        public bool RemoveDeck(string id) => _gwentServerService.RemoveDeck(Context.ConnectionId, id);

        //修改卡组
        public bool ModifyDeck(string id, DeckModel deck) => _gwentServerService.ModifyDeck(Context.ConnectionId, id, deck);
        public bool ModifyBlacklist(List<string> blacklist) => _gwentServerService.ModifyBlacklist(Context.ConnectionId, blacklist);


        //开始匹配(老api),算作空密码匹配
        public bool Match(string deckId) => MatchOfPassword(deckId, string.Empty);

        //使用密码匹配
        public bool MatchOfPassword(string deckId, string password) => _gwentServerService.Match(Context.ConnectionId, deckId, password);

        //停止匹配
        public async Task<bool> StopMatch() => await _gwentServerService.StopMatch(Context.ConnectionId);
        public bool Surrender() => _gwentServerService.Surrender(Context.ConnectionId); // 投降

        public string GetCardMapVersion()
        {
            return GwentMap.CardMapVersion.ToString();
        }

        public string GetCardMap() => _gwentServerService.GetCardMap();
        public string GetGameLocales() => _gwentServerService.GetGameLocales();

        public async Task<string> GetLatestVersion() => await _gwentServerService.GetLatestVersion(Context.ConnectionId);

        public async Task<string> GetNotes() => await _gwentServerService.GetNotes(Context.ConnectionId);

        //获取在线人数
        public async Task<int> GetUserCount()
        {
            await Task.CompletedTask;
            return _gwentServerService.GetUserCount();//(Context.ConnectionId);
        }

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