using System.Collections.Generic;
using System.Linq;
using Alsein.Utilities.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using Alsein.Utilities.IO;

namespace Cynthia.Card.Server
{
    [Singleton]
    public class GwentServerService
    {
        public IContainer Container { get; set; }
        public GwentDatabaseService DatabaseService { get; set; }
        private readonly GwentMatchs _gwentMatchs;
        private readonly IDictionary<string, User> _users = new Dictionary<string, User>();
        private readonly IDictionary<string,(IAsyncDataSender sender,IAsyncDataReceiver receiver)> _waitReconnectList = new Dictionary<string,(IAsyncDataSender,IAsyncDataReceiver)>();
        public GwentServerService(IContainer container,GwentDatabaseService databaseService)
        {
            Container = container;
            DatabaseService = databaseService;
            _gwentMatchs = new GwentMatchs(Container.Resolve<IHubContext<GwentHub>>);
        }
        public async Task<UserInfo> Login(User user, string password)
        {
            //判断登录条件
            var loginUser = DatabaseService.Login(user.UserName, password);
            if (loginUser != null)
            {
                if (_users.Any(x => x.Value.UserName == user.UserName))//如果重复登录的话,触发"掉线"
                {
                    var connectionId = _users.Single(x => x.Value.UserName == user.UserName).Value.ConnectionId;
                    await Container.Resolve<IHubContext<GwentHub>>().Clients.Client(connectionId).SendAsync("RepeatLogin");
                    await Disconnect(connectionId);
                }
                if (_users.ContainsKey(user.ConnectionId))
                {
                    await Disconnect(user.ConnectionId);
                }
                user.PlayerName = loginUser.PlayerName;
                user.Decks = loginUser.Decks;
                _users.Add(user.ConnectionId, user);
            }
            return loginUser;
        }
        public bool Register(string username, string password, string playerName) => DatabaseService.Register(username, password, playerName);
        public bool Match(string connectionId, int deckIndex)//匹配
        {
            if (_users.ContainsKey(connectionId))
            {
                var user = _users[connectionId];
                if (user.UserState != UserState.Standby || user.Decks.Count <= deckIndex || deckIndex < 0)
                    return false;
                var player = user.CurrentPlayer = new ClientPlayer(user, Container.Resolve<IHubContext<GwentHub>>);
                player.Deck = user.Decks[deckIndex];
                _gwentMatchs.PlayerJoin(player);
                return true;
            }
            return false;
        }
        public async Task<bool> StopMatch(string connectionId)
        {
            if (_users[connectionId].UserState != UserState.Match)
            {
                return false;
            }
            return await _gwentMatchs.StopMatch(connectionId);
        }
        public bool AddDeck(string connectionId, DeckModel deck)
        {
            //添加卡组
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count >= 40)
                return false;
            if (!deck.IsBasicDeck())
                return false;
            if (!DatabaseService.AddDeck(user.UserName, deck))
                return false;
            user.Decks.Add(deck);
            return true;
        }
        public bool RemoveDeck(string connectionId, int deckIndex)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count <= deckIndex || deckIndex < 0)
                return false;
            if (!DatabaseService.RemoveDeck(user.UserName, deckIndex))
                return false;
            user.Decks.RemoveAt(deckIndex);
            return true;
        }
        public bool ModifyDeck(string connectionId, int deckIndex, DeckModel deck)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count <= deckIndex || deckIndex < 0)
                return false;
            if (!deck.IsBasicDeck())
                return false;
            if (!DatabaseService.ModifyDeck(user.UserName, deckIndex, deck))
                return false;
            user.Decks[deckIndex] = deck;
            return true;
        }
        public Task GameOperation(Operation<UserOperationType> operation, string connectionId) => _users[connectionId].CurrentPlayer.SendAsync(operation);
        public async Task Disconnect(string connectionId,Exception exception = null,bool isWaitReconnect = false)
        {
            if (!_users.ContainsKey(connectionId))//如果用户没有在线,无效果
                return;
            if (_users[connectionId].UserState == UserState.Match)//如果用户正在匹配
            {
                _ = _gwentMatchs.StopMatch(connectionId);//停止匹配
            }
            if(isWaitReconnect)
            {
                if(_users[connectionId].UserState == UserState.Play)
                {
                    await _gwentMatchs.WaitReconnect(connectionId,()=>WaitReconnect(connectionId));
                }
                else
                {
                    await WaitReconnect(connectionId);
                }
            }
            else
            {
                if (_users[connectionId].UserState == UserState.Play)//如果用户正在进行对局
                {
                    _gwentMatchs.PlayerLeave(connectionId,exception);
                }
                _users.Remove(connectionId);
                _waitReconnectList.Remove(connectionId);
            }

        }
        public async Task<bool> WaitReconnect(string connectionId)
        {   //等待重连
            if(!_users.ContainsKey(connectionId)) return false;
            //如果没有发现链接,重连失败
            _users[connectionId].IsWaitingReConnect = true;
            _waitReconnectList[_users[connectionId].UserName] = AsyncDataEndPoint.CreateSimplex();
            //建立管道,键为用户名
            var timeOverTask = Task.Delay(10000);
            var connectTask = _waitReconnectList[_users[connectionId].UserName].receiver.ReceiveAsync<bool>();
            switch(await Task.WhenAny(timeOverTask,connectTask))
            {
                case Task<bool> task when task == connectTask:
                    return true;
                case Task task when task == timeOverTask:
                default://如果时间结束或者出现了奇怪的结果
                    _users.Remove(connectionId);
                    _waitReconnectList.Remove(connectionId);
                    return false;
            }
        }
        public async Task<bool> Reconnect(string connectionId,string userName, string password)
        {
            //如果等待重连列表里面没有的话,重连失败,请重新登陆游戏
            if(!_waitReconnectList.ContainsKey(userName)) return false;
            var user = DatabaseService.Login(userName, password);
            if(user==null||!_users.Any(x=>x.Value.UserName==userName))return false; //如果重连身份验证失败,自然不允许
            var nowUser = _users.Single(x=>x.Value.UserName==userName).Value;
            if(!nowUser.IsWaitingReConnect)return false;
            nowUser.IsWaitingReConnect = false;
            _users.Remove(_users.Single(x=>x.Value.UserName==userName).Key);
            nowUser.ConnectionId = connectionId;
            _users[connectionId] = nowUser;
            //替换链接
            await _waitReconnectList[userName].sender.SendAsync<bool>(true);
            return true;
        }
    }
}