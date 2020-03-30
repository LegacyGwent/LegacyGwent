using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions.LifetimeAnnotations;
using Autofac;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using System;
using Alsein.Extensions.IO;
using System.Collections.Concurrent;
using Alsein.Extensions;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Cynthia.Card.Server
{
    [Singleton]
    public class GwentServerService
    {
        //public IContainer Container { get; set; }
        private readonly IHubContext<GwentHub> _hub;
        public GwentDatabaseService _databaseService;
        private readonly GwentMatchs _gwentMatchs;

        private GwentCardDataService _gwentCardDataService;
        public IWebHostEnvironment _env;
        private readonly IDictionary<string, User> _users = new ConcurrentDictionary<string, User>();
        // private readonly IDictionary<string, (ITubeInlet sender, ITubeOutlet receiver)> _waitReconnectList = new ConcurrentDictionary<string, (ITubeInlet, ITubeOutlet)>();
        public GwentServerService(IHubContext<GwentHub> hub, GwentDatabaseService databaseService, IServiceProvider container, IWebHostEnvironment env, GwentCardDataService gwentCardDataService)
        {
            //Container = container;
            _databaseService = databaseService;
            _gwentMatchs = new GwentMatchs(() => hub, (GwentCardDataService)container.GetService(typeof(GwentCardDataService)), this);
            _hub = hub;
            _env = env;
            ResultList = _databaseService.GetAllGameResults(50);
            _gwentCardDataService = gwentCardDataService;
        }

        public async Task<UserInfo> Login(User user, string password)
        {
            //判断用户名与密码
            var loginUser = _databaseService.Login(user.UserName, password);
            if (loginUser != null)
            {
                if (_users.Any(x => x.Value.UserName == user.UserName))//如果重复登录的话,触发"掉线"
                {
                    var connectionId = _users.Single(x => x.Value.UserName == user.UserName).Value.ConnectionId;
                    await _hub.Clients.Client(connectionId).SendAsync("RepeatLogin");
                    await Disconnect(connectionId);
                }
                if (_users.ContainsKey(user.ConnectionId))
                {
                    await Disconnect(user.ConnectionId);
                }
                user.PlayerName = loginUser.PlayerName;
                user.Decks = loginUser.Decks;
                _users.Add(user.ConnectionId, user);
                InovkeUserChanged();
            }
            return loginUser;
        }

        public bool Register(string username, string password, string playerName) => _databaseService.Register(username, password, playerName);

        public bool Match(string connectionId, string deckId, string password)//匹配
        {
            //如果这个玩家在登陆状态,并且处于闲置中
            if (_users.ContainsKey(connectionId) && _users[connectionId].UserState == UserState.Standby)
            {
                //获取这个玩家
                var user = _users[connectionId];
                //如果玩家不处于闲置状态,或玩家没有该Id的卡组,或者该卡组不符合标准,禁止匹配
                if (user.UserState != UserState.Standby || !(user.Decks.Any(x => x.Id == deckId) && user.Decks.Single(x => x.Id == deckId).IsBasicDeck()))
                    return false;
                //建立一个新的玩家
                var player = user.CurrentPlayer = new ClientPlayer(user, () => _hub);//Container.Resolve<IHubContext<GwentHub>>);
                //设置玩家的卡组
                player.Deck = user.Decks.Single(x => x.Id == deckId);
                //将这个玩家加入到游戏匹配系统之中
                _gwentMatchs.PlayerJoin(player, password);
                InovkeUserChanged();
                //成功进入匹配队列了哟
                return true;
            }
            //玩家未在线,失败
            return false;
        }

        public async Task<bool> StopMatch(string connectionId)
        {
            if (_users[connectionId].UserState != UserState.Match && _users[connectionId].UserState != UserState.PasswordMatch)
            {
                return false;
            }
            var result = await _gwentMatchs.StopMatch(connectionId);
            InovkeUserChanged();
            return result;
        }

        public bool AddDeck(string connectionId, DeckModel deck)
        {
            //添加卡组
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count >= 40)
                return false;
            //if (!deck.IsBasicDeck())
            //return false;
            if (!_databaseService.AddDeck(user.UserName, deck))
                return false;
            user.Decks.Add(deck);
            return true;
        }

        public bool RemoveDeck(string connectionId, string id)
        {
            //如果用户不处于登陆状态,拒绝删除卡组
            if (!_users.ContainsKey(connectionId))
                return false;
            //获取用户
            var user = _users[connectionId];
            //如果用户的卡组数量小于0,拒绝删除卡组
            if (user.Decks.Count < 0)
                return false;
            if (user.Decks.Any(x => x.Id == id))
                if (!_databaseService.RemoveDeck(user.UserName, id))
                    return false;
            user.Decks.RemoveAt(user.Decks.Select((x, index) => (x, index)).Single(deck => deck.x.Id == id).index);
            return true;
        }

        public bool ModifyDeck(string connectionId, string id, DeckModel deck)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count < 0)
                return false;
            //如果卡组不合规范
            if (!_databaseService.ModifyDeck(user.UserName, id, deck))
                return false;
            user.Decks[user.Decks.Select((x, index) => (x, index)).Single(d => d.x.Id == id).index] = deck;
            return true;
        }

        public Task GameOperation(Operation<UserOperationType> operation, string connectionId)
        {
            var result = _users[connectionId].CurrentPlayer.SendAsync(operation);
            return result;
        }

        public async Task Disconnect(string connectionId, Exception exception = null)//, bool isWaitReconnect = false)
        {
            await Task.CompletedTask;
            if (!_users.ContainsKey(connectionId))//如果用户没有在线,无效果
                return;
            if (_users[connectionId].UserState == UserState.Match)//如果用户正在匹配
            {
                _ = _gwentMatchs.StopMatch(connectionId);//停止匹配
            }
            if (_users[connectionId].UserState == UserState.Play)//如果用户正在进行对局
            {
                _gwentMatchs.PlayerLeave(connectionId, exception);
            }
            _users.Remove(connectionId);
            InovkeUserChanged();
        }

        public async Task<string> GetLatestVersion(string connectionId)
        {
            await Task.CompletedTask;
            return "0.1.0.1";
        }

        public async Task<string> GetNotes(string connectionId)
        {
            await Task.CompletedTask;
            return @"这里是是DIY-PTR服务器,祝大家玩得开心~
查看实时在线人数可查网站http://cynthia.ovyno.com:5005
欢迎加群闲聊约战~关注第一消息
群号:945408322
本作永久免费开源,详细欢迎入群了解:)

注意事项: 
1. 账号密码与原服务器分开，需要重新注册
2. 游戏中有可能断线、更新内容
3. 全部更新内容请参照https://shimo.im/docs/TQdjjwpPwd9hJhK
    （群公告中可直接点开链接）

3.30日更新
修正茜格德莉法，伯爵，薇薇安，攻城后援的bug，
削弱格莱尼斯·爱普·洛纳克，哈蒙德，湖中仙女，
实装昆恩印记。
详细更新内容请看上面的石墨文档链接
";
        }
        //-------------------------------------------------------------------------
        public int GetUserCount()
        {
            return _users.Count;
        }

        public void InovkeUserChanged()
        {
            OnUserChanged?.Invoke(GetUsers());
        }

        public IList<GameResult> ResultList { get; private set; } = new List<GameResult>();

        public void InvokeGameOver(GameResult result)
        {
            if (_env.IsProduction())
            {
                if (_databaseService.AddGameResult(result))
                {
                    lock (ResultList)
                    {
                        ResultList.Add(result);
                    }
                }
                OnGameOver?.Invoke(result);
            }
        }

        public (IList<IGrouping<UserState, User>>, IList<(string, string)>) GetUsers()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady).Select(x => (x.Player1.CurrentUser.PlayerName, x.Player2.CurrentUser.PlayerName)).ToList();
            return (_users.Select(x => x.Value).Where(x => x.UserState != UserState.Play).GroupBy(x => x.UserState).ToList(), list);
        }

        public event Action<(IList<IGrouping<UserState, User>>, IList<(string, string)>)> OnUserChanged;

        public event Action<GameResult> OnGameOver;

        public string GetCardMap()
        {
            return _gwentCardDataService.GetCardMap();
        }
    }
}
