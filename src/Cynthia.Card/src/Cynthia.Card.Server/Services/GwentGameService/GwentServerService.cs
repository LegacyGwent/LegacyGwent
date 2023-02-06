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
using Cynthia.Card.AI;
using Cynthia.Card.Server.Services.GwentGameService;

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
        private GwentLocalizationService _gwentLocalizationService;

        public IWebHostEnvironment _env;
        private readonly IDictionary<string, User> _users = new ConcurrentDictionary<string, User>();
        // private readonly IDictionary<string, (ITubeInlet sender, ITubeOutlet receiver)> _waitReconnectList = new ConcurrentDictionary<string, (ITubeInlet, ITubeOutlet)>();
        public GwentServerService(
            IHubContext<GwentHub> hub,
            GwentDatabaseService databaseService,
            IServiceProvider container,
            IWebHostEnvironment env,
            GwentCardDataService gwentCardDataService,
            GwentLocalizationService gwentLocalizationService
        )
        {
            _databaseService = databaseService;
            _gwentMatchs = new GwentMatchs(() => hub, (GwentCardDataService)container.GetService(typeof(GwentCardDataService)), this);
            _hub = hub;
            _env = env;
            ResultList = _databaseService.GetAllGameResults(50);
            _gwentCardDataService = gwentCardDataService;
            _gwentLocalizationService = gwentLocalizationService;
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
                user.Blacklist = loginUser.Blacklist;
                _users.Add(user.ConnectionId, user);
                InovkeUserChanged();
            }
            return loginUser;
        }

        public bool Register(string username, string password, string playerName) => _databaseService.Register(username, password, playerName);

        public bool Match(string connectionId, string deckId, string password, int usingBlacklist)//匹配
        {
            //如果这个玩家在登陆状态,并且处于闲置中
            if (_users.ContainsKey(connectionId) && _users[connectionId].UserState == UserState.Standby)
            {
                //获取这个玩家
                var user = _users[connectionId];
                //如果玩家不处于闲置状态,或玩家没有该Id的卡组,或者该卡组不符合标准,禁止匹配
                if (user.UserState != UserState.Standby || !(user.Decks.Any(x => x.Id == deckId) && (user.Decks.Single(x => x.Id == deckId).IsSpecialDeck() || user.Decks.Single(x => x.Id == deckId).IsBasicDeck())))
                    return false;
                //建立一个新的玩家
                var player = user.CurrentPlayer = new ClientPlayer(user, () => _hub);//Container.Resolve<IHubContext<GwentHub>>);
                //设置玩家的卡组
                player.Deck = user.Decks.Single(x => x.Id == deckId);
                if (usingBlacklist == 1)
                    player.Blacklist = user.Blacklist;
                else
                    player.Blacklist = null;

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

        public bool Surrender(string connectionId) // 投降
        {
            var result = _gwentMatchs.PlayerLeave(connectionId, new Exception("已投降\nSurrendered"), isSurrender: true);
            InovkeUserChanged();
            return result;
        }

        public bool JoinViewList(string connectionId, string roomId)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.UserState != UserState.Standby)
                return false;
            if (!_gwentMatchs.JoinViewList(user, roomId))
                return false;
            user.UserState = UserState.Viewing;
            InovkeUserChanged();
            return true;
        }

        public bool LeaveViewList(string connectionId, string roomId = "")
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.UserState != UserState.Viewing)
                return false;
            if (!_gwentMatchs.LeaveViewList(user, roomId))
                return false;
            user.UserState = UserState.Standby;
            InovkeUserChanged();
            return true;
        }

        public bool AddDeck(string connectionId, DeckModel deck)
        {
            //添加卡组
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count >= 100)
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

        public bool ModifyBlacklist(string connectionId, BlacklistModel blacklist)
        {
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count < 0)
                return false;
            //如果黑名单不合规范
            if (!_databaseService.ModifyBlacklist(user.UserName, blacklist))
                return false;
            user.Blacklist = blacklist;
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
            if (_users[connectionId].UserState == UserState.Match || _users[connectionId].UserState == UserState.PasswordMatch)//如果用户正在匹配
            {
                _ = _gwentMatchs.StopMatch(connectionId);//停止匹配
            }
            if (_users[connectionId].UserState == UserState.Play || _users[connectionId].UserState == UserState.PlayWithAI)//如果用户正在进行对局
            {
                _gwentMatchs.PlayerLeave(connectionId, exception);
            }
            if (_users[connectionId].UserState == UserState.Viewing)//如果用户正在观战
            {
                _gwentMatchs.LeaveViewList(_users[connectionId], "");
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
本作永久免费开源,详细欢迎入群了解

输入密码ai/ai1/ai2即可挑战ai~(当有其他人匹配时优先玩家匹配)
ai密码后缀#f(如ai#f)即可强制挑战ai,不会进行玩家匹配

注意事项: 
1. 账号密码与原服务器分开，需要重新注册
2. 游戏中有可能断线、更新内容
3. 全部更新内容请参照https://shimo.im/docs/TQdjjwpPwd9hJhK
    （群公告中可直接点开链接）

2023年2月6日更新
卡牌调整：
伊勒瑞斯：临终之日~决斗仅生效3次
斯崔葛布~添加力竭
亚伯力奇~添加力竭
希拉德~添加力竭
安德莱格虫卵~本体改为3点召唤1张同名牌，衍生物为5点和7点
拉多维德~伤害4->5
亚托列司·薇歌~战力11->9

卡牌替换：
解梦术替换为神灯

新卡：
神灯~中立金，对局开始时，将3张“最后的愿望”加入卡组，随后丢弃自身
疯狂的冲锋~北方铜，使1个受护甲保护的友军单位与1个敌军单位对决

功能性调整：
为所有无计数器的力竭牌，添加一个计数为1的计数器

描述修正：
假死、烟火技师、阻魔金镣铐

2022年1月24日更新
删除雷蒂娅的晋升效果。雷蒂娅将重做。晋升卡可能会在以后以其他方式回归。
Delete the promotion effect of Radeyah. Promotion cards
may come back in the future.

2020年8月21日更新(Charli)
北方新卡: 战象
10余项老卡平衡修改[详细见群公告]

2020年8月21日更新
1.新的ai - 爆牌ai (输入ai2进行对战
2.改动
亚克席： 效果作用于 -> 添加限制,无法选择3个单位以下排的单位
科沃的维索戈塔： 只有进行增益时,才会对自身造成伤害

2020年8月19日更新
1.4张新卡
中立铜卡:鬼针草煎药,合欢茎魔药
群岛铜卡:恐狼持斧者,恐狼勇士
2.改动亚克席：
效果作用于 -> (敌方忠诚铜色/银色)=>(敌方忠诚非领袖)

2020年7月24日更新
1.改动卡牌科沃的维索戈塔
战力 -> 6
效果 -> 回合开始时，左侧单位获得3点增益，自身受到1点伤害，并移至己方单位最少排。遗愿：己方场上最弱单位获得6点增益。

2020年6月28日更新
1.贝哈文的伊沃 -> 增益改为强化, 帝国改为中立。
2.格莱尼斯·爱普·洛纳克 -> 新增佚亡, 修复在牌库底不跳的bug。
3.先知雷比欧达 -> 品质金变为品质银。


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

        public void InvokeGameOver(GameResult result, bool isOnlyShow, bool isCountMMR)
        {
            // if (_env.IsProduction())
            // {
            if (isOnlyShow)
            {
                _databaseService.AddAIGameResult(result);
            }
            else
            {
                _databaseService.AddGameResult(result);
            }

            if (isCountMMR)
            {
                int RedMMR = _databaseService.QueryMMR(result.RedPlayerName);
                int BlueMMR = _databaseService.QueryMMR(result.BluePlayerName);
                RedMMR = CalculateMMR(RedMMR, BlueMMR,
                    result.RedPlayerGameResultStatus == GameStatus.Win,
                    result.RedPlayerGameResultStatus == GameStatus.Draw);
                BlueMMR = CalculateMMR(BlueMMR, RedMMR,
                    result.RedPlayerGameResultStatus == GameStatus.Lose,
                    result.RedPlayerGameResultStatus == GameStatus.Draw);
                _databaseService.UpdateMMR(result.RedPlayerName, Math.Max(RedMMR, 0));
                _databaseService.UpdateMMR(result.BluePlayerName, Math.Max(BlueMMR, 0));
            }
            lock (ResultList)
            {
                ResultList.Add(result);
            }
            OnGameOver?.Invoke(result);
            // }
        }

        public int CalculateMMR(int myMMR, int enemyMMR, bool isWin, bool isDraw)
        {
            int k = CalculateK(myMMR);
            double s = isDraw ? 0.5 : (isWin ? 1 : 0);
            int diff = Math.Max(enemyMMR - myMMR, -50);
            double e = 1 / (1.0 + Math.Pow(10, diff / 400.0));
            double eta = CalculateEta(s, e, myMMR);
            int addon = myMMR < 2000 ? (isWin ? 70 : 50) : 0;
            return (int)Math.Round(myMMR + eta * k * (s - e) + addon);
        }

        public int CalculateK(int MMR)
        {
            if (MMR < 3079)
            {
                return 100;
            }
            else if (MMR < 3439)
            {
                return 80;
            }
            else if (MMR < 3709)
            {
                return 70;
            }
            else if (MMR < 4029)
            {
                return 60;
            }
            else if (MMR < 4259)
            {
                return 45;
            }
            else
            {
                return 30;
            }
        }
        public double CalculateEta(double s, double e, int MMR)
        {
            if (s > e)
            {
                return 1;
            }
            else
            {
                if (MMR < 1000)
                {
                    return 0;
                }
                else if (MMR < 4000)
                {
                    return 0.8 * (0.81 * ((MMR - 1000.0) / 3000) * ((MMR - 1000.0) / 3000) + 0.19 * ((MMR - 1000.0) / 3000));
                }
                else
                {
                    return 1;
                }
            }
        }

        public (IList<IGrouping<UserState, User>>, IList<(string, string)>, IList<(string, string)>) GetUsers()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            var aiList = _gwentMatchs.GwentRooms.Where(x => x.IsReady && (x.Player1 is AIPlayer || x.Player2 is AIPlayer)).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return (_users.Select(x => x.Value).Where(x => x.UserState != UserState.Play && x.UserState != UserState.PlayWithAI).GroupBy(x => x.UserState).ToList(), list, aiList);
        }

        public (IList<IGrouping<UserState, User>>, IList<(string, string, string)>, IList<(string, string, string)>) GetUsersWithRoomId()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName, x.RoomId)).ToList();
            var aiList = _gwentMatchs.GwentRooms.Where(x => x.IsReady && (x.Player1 is AIPlayer || x.Player2 is AIPlayer)).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName, x.RoomId)).ToList();
            return (_users.Select(x => x.Value).Where(x => x.UserState != UserState.Play && x.UserState != UserState.PlayWithAI).GroupBy(x => x.UserState).ToList(), list, aiList);
        }

        public event Action<(IList<IGrouping<UserState, User>>, IList<(string, string)>, IList<(string, string)>)> OnUserChanged;

        public event Action<GameResult> OnGameOver;

        public string GetCardMap()
        {
            return _gwentCardDataService.GetCardMap();
        }

        public string GetGameLocales()
        {
            return _gwentLocalizationService.GetGameLocales();
        }

        public int GetPalyernameMMR(string playername) => _databaseService.QueryMMR(playername);

        public IList<Tuple<string, int>> GetAllMMR(int offset, int limit) => _databaseService.QueryAllMMR(offset, limit);
    }
}
