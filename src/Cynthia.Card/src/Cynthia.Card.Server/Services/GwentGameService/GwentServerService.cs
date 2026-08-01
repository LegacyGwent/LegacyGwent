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
using Cynthia.Card.Common.Models;
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
            UpdateAndSaveSeasons();
            
        }

        //for ongoing season
        private void CreatePlayersStreaksFromGameResults(DateTime minDate, DateTime? maxDate = null)
        {
            Dictionary<Faction, int> factionIndexMap = new Dictionary<Faction, int>
            {
                { Faction.Monsters, 0 },
                { Faction.Nilfgaard, 1 },
                { Faction.NorthernRealms, 2 },
                { Faction.ScoiaTael, 3 },
                { Faction.Skellige, 4 },
            };

            const int WINS = 0;
            const int DEFEATS = 1;
            const int DRAWS = 2;

            var results = _databaseService.GetAllGameResults(30000).OrderByDescending(result => result.Time).ToList();

            var players = _databaseService.GetAllPlayers();

            var playersStreaks = new Dictionary<string, List<int[]>>();

            foreach (var gameresult in results)
            {

                if (!gameresult.IsEffective() || !gameresult.isRanked)
                    continue;

                if (maxDate.HasValue && gameresult.Time > maxDate)
                    continue;

                if (gameresult.Time < minDate)
                    break;


                string redPlayer = gameresult.RedPlayerName;
                int redPlayerFaction = factionIndexMap.ContainsKey(GwentMap.CardMap[gameresult.RedLeaderId].Faction)
                    ? factionIndexMap[GwentMap.CardMap[gameresult.RedLeaderId].Faction]
                    : -1;  
                string bluePlayer = gameresult.BluePlayerName;
                int bluePlayerFaction = factionIndexMap.ContainsKey(GwentMap.CardMap[gameresult.BlueLeaderId].Faction)
                    ? factionIndexMap[GwentMap.CardMap[gameresult.BlueLeaderId].Faction]
                    : -1;

                if (redPlayerFaction == -1 || bluePlayerFaction == -1)
                    continue;

                if (!playersStreaks.ContainsKey(redPlayer))
                    {
                        playersStreaks[redPlayer] = new List<int[]>();
                        for (int i = 0; i < 5; i++)
                        {
                            playersStreaks[redPlayer].Add(new int[] { 0, 0, 0 });
                        }
                    }
                if (!playersStreaks.ContainsKey(bluePlayer))
                {
                    playersStreaks[bluePlayer] = new List<int[]>();
                    for (int i = 0; i < 5; i++)
                    {
                        playersStreaks[bluePlayer].Add(new int[] { 0, 0, 0 });
                    }
                }

                switch (gameresult.RedPlayerGameResultStatus)
                {
                    case GameStatus.Win:
                        playersStreaks[redPlayer][redPlayerFaction][WINS]++;
                        playersStreaks[bluePlayer][bluePlayerFaction][DEFEATS]++;
                        break;
                    case GameStatus.Lose:
                        playersStreaks[redPlayer][redPlayerFaction][DEFEATS]++;
                        playersStreaks[bluePlayer][bluePlayerFaction][WINS]++;
                        break;
                    default:
                        playersStreaks[redPlayer][redPlayerFaction][DRAWS]++;
                        playersStreaks[bluePlayer][bluePlayerFaction][DRAWS]++;
                        break;
                }
            }
            
            foreach (var _playerStreak in playersStreaks)
            {
                _databaseService.SetStreak(_playerStreak.Key, _playerStreak.Value);
            }

        }

        public IList<SeasonInfo> GetSeasons()
        {
            var c = _databaseService.QuerySeasons();
            return c;
        }
        
        /// <summary>Updates only: { NAME, ENDTIME, REWARDS } of season with existing ID or creates new.</summary>
        private Task UpdateAndSaveSeasons()
        {

            List<SeasonReward> season2Rewards = new List<SeasonReward>()
            {
                new SeasonReward(minimalPosition: 1, border: "Season2Border6", title: "PROTECTOR"),
                new SeasonReward(minimalPosition: 5, border: "Season2Border5", title: "DEFENDER"),
                new SeasonReward(minimalPosition: 10, border: "Season2Border4", title: "REBEL"),
                new SeasonReward(minimalPosition: 20, border: "Season2Border3", title: "HUNTER"),
                new SeasonReward(minimalPosition: 30, avatar: "Iorveth"),
                new SeasonReward(minimalPosition: 50, border: "Season2Border2", title: "TRAPPER"),
                new SeasonReward(minimalPosition: 100, border: "Season2Border1", title: "RANGER")
            };
            List<SeasonReward> season3Rewards = new List<SeasonReward>()
            {
                new SeasonReward(minimalPosition: 100, border: "Season3Border6", title: "DRAGONHATCHLING"),
                new SeasonReward(minimalPosition: 50, border: "Season3Border5", title: "YOUNGDRAKE"),
                new SeasonReward(minimalPosition: 30, border: "Season3Border4", title: "SLYZARD"),
                new SeasonReward(minimalPosition: 20, border: "Season3Border3", title: "WYVERN"),
                new SeasonReward(minimalPosition: 10, avatar: "Geralt_Intoxicated"),
                new SeasonReward(minimalPosition: 5, border: "Season3Border2", title: "GREATWYRM"),
                new SeasonReward(minimalPosition: 1, border: "Season3Border1", title: "GOLDENDRAGON"),

                // In-season rewards (MMR based) for Season 3:
                // - Avatar at "rank 10" threshold
                // - Avatar at "rank 18" threshold
                // These thresholds currently correspond to 3850 and 4250 MMR.
                new SeasonReward(minimalPosition: 0, avatar: "Zoltan_Animal_Tamer", isInSeasonReward: true, minimalMMR: 3850),
                new SeasonReward(minimalPosition: 0, avatar: "EredinMasked", isInSeasonReward: true, minimalMMR: 4250)
            };
            List<SeasonReward> season4Rewards = new List<SeasonReward>()
            {
                new SeasonReward(minimalPosition: 100, border: "Season4Border1", title: "WILDHUNTHOUND"),
                new SeasonReward(minimalPosition: 50, border: "Season4Border2", title: "WILDHUNTWARRIOR"),
                new SeasonReward(minimalPosition: 30, border: "Season4Border3", title: "WILDHUNTRIDER"),
                new SeasonReward(minimalPosition: 20, border: "Season4Border4", title: "WILDHUNTNAVIGATOR"),
                new SeasonReward(minimalPosition: 10, avatar: "Letho"),
                new SeasonReward(minimalPosition: 5, border: "Season4Border5", title: "WILDHUNTGENERAL"),
                new SeasonReward(minimalPosition: 1, border: "Season4Border6", title: "WILDHUNTKING"),

                // In-season rewards (MMR based) for Season 4:  
                // - Avatar at "rank 10" threshold
                // - Avatar at "rank 18" threshold
                // These thresholds currently correspond to 3850 and 4250 MMR.
                new SeasonReward(minimalPosition: 0, avatar: "Radovid", isInSeasonReward: true, minimalMMR: 3850),
                new SeasonReward(minimalPosition: 0, avatar: "Vernon_Roche", isInSeasonReward: true, minimalMMR: 4250)
            };
            List<SeasonReward> season5Rewards = new List<SeasonReward>()
            {
                new SeasonReward(minimalPosition: 100, border: "Season5Border1", title: "WOLFPUP"),   
                new SeasonReward(minimalPosition: 50, border: "Season5Border2", title: "YOUNGWOLF"),
                new SeasonReward(minimalPosition: 30, border: "Season5Border3", title: "DENPROTECTOR"),
                new SeasonReward(minimalPosition: 20, border: "Season5Border4", title: "PACKLEADER"),
                new SeasonReward(minimalPosition: 10, avatar: "Dagon"),
                new SeasonReward(minimalPosition: 5, border: "Season5Border5", title: "GRANDWARG"),
                new SeasonReward(minimalPosition: 1, border: "Season5Border6", title: "WHITEWOLF"),

                // In-season rewards (MMR based) for Season 5:  
                // - Avatar at "rank 10" threshold
                // - Avatar at "rank 18" threshold
                // These thresholds currently correspond to 3850 and 4250 MMR.
                new SeasonReward(minimalPosition: 0, avatar: "Francesca", isInSeasonReward: true, minimalMMR: 3850),
                new SeasonReward(minimalPosition: 0, avatar: "King_Bran", isInSeasonReward: true, minimalMMR: 4250)
            };


            List<Season> seasonsList = new List<Season>
            {
                new Season() { id = 1, name = "Season_SeasonOfTheDragon", endTime = new DateTime(2025, 12, 27, 0, 0, 0, DateTimeKind.Utc), color = "orange", seasonalRewards = season3Rewards },
                new Season() { id = 2, name = "Season_WildHuntSeason", endTime = new DateTime(2026, 06, 27, 0, 0, 0, DateTimeKind.Utc), color = "lightblue", seasonalRewards = season4Rewards },
                new Season() { id = 3, name = "Season_WolfSeason", endTime = new DateTime(2026, 09, 27, 0, 0, 0, DateTimeKind.Utc), color = "blue", seasonalRewards = season5Rewards }
            }; 
            // Season 6 will be added later TO DO: decide if repeat season 1 or go into Homecoming rewards
            return _databaseService.UpdateSeasons(seasonsList);
        }

        public async Task<bool> GiveAwaySeasonalRewards()
        {
            // Get top players by MMR
            var topPlayers = _databaseService.GetAllPlayers()
                .OrderByDescending(p => p.MMR)
                .ToList();



            // Faction ranking awards
            var rankList = GetAllMMRExtended(0, 300);
            var factionsRankList = Season.CalculateFactions(rankList, 300);

            var playerFactionsRankRewardsBorders = new Dictionary<string, List<string>>();
            var playerFactionsRankRewardsTitles = new Dictionary<string, List<string>>();


            var seasonRewards = GetSeasonRewards(-1);
            
            var factionBorders = new List<string>() { "FactionMO", "FactionNG", "FactionNR", "FactionST", "FactionSK" };
            var factionTitles = new List<string>() { "MONSTER", "NILFGAARDIAN", "NORTHERNER", "SCOIA'TAEL", "SKELLIGER" };

            foreach (var factionRanking in factionsRankList)
            {
                int bordersRewardsLeft = 3;
                int titlesRewardsLeft = 10;

                for (int i = 0; i < factionRanking.Value.Count; i++)
                {
                    if (bordersRewardsLeft < 1 && titlesRewardsLeft < 1)
                        break;

                    var playerRank = i + 1;
                    var player = factionRanking.Value[i].Item1;

                    if (bordersRewardsLeft > 0)
                    {
                        string _borderReward = factionBorders[factionRanking.Key];
                        bool newTrinket = await AddBorder(player, _borderReward);
                        if (newTrinket)
                        {
                            bordersRewardsLeft--;
                            if (!playerFactionsRankRewardsBorders.ContainsKey(player))
                                playerFactionsRankRewardsBorders[player] = new List<string>();
                            playerFactionsRankRewardsBorders[player].Add(_borderReward);

                        }
                    }
                    
                    if (titlesRewardsLeft > 0)
                    {
                        string _titleReward = factionTitles[factionRanking.Key];
                        bool newTrinket = await AddTitle(player, _titleReward);
                        if (newTrinket)
                        {
                            titlesRewardsLeft--;
                            if (!playerFactionsRankRewardsTitles.ContainsKey(player))
                                playerFactionsRankRewardsTitles[player] = new List<string>();
                            playerFactionsRankRewardsTitles[player].Add(_titleReward);
                        }
                    }


                }
            }

            // Award rewards to top players
            for (int i = 0; i < topPlayers.Count; i++)
            {
                var player = topPlayers[i];
                var rank = i + 1;

                List<string> avatarRewards = new List<string>();
                List<string> borderRewards = new List<string>(playerFactionsRankRewardsBorders.GetValueOrDefault(player.PlayerName) ?? new List<string>());
                List<string> titleRewards = new List<string>(playerFactionsRankRewardsTitles.GetValueOrDefault(player.PlayerName) ?? new List<string>());

                


                var playerSeasonsRewards = seasonRewards.Where(x => x.minimalPosition > 0).ToList();

                foreach (var seasonReward in playerSeasonsRewards)
                {
                    if (rank > seasonReward.minimalPosition)
                        continue;
                    if (seasonReward.avatar != null)
                    {
                        avatarRewards.Add(seasonReward.avatar);
                        await AddAvatar(player.PlayerName, seasonReward.avatar);
                    }
                    if (seasonReward.border != null)
                    {
                        borderRewards.Add(seasonReward.border);
                        await AddBorder(player.PlayerName, seasonReward.border);
                    }
                    if (seasonReward.title != null)
                    {
                        titleRewards.Add(seasonReward.title);
                        await AddTitle(player.PlayerName, seasonReward.title);
                    }
                }

                var season_data = await GetSeasonData();

                await SendSeasonEndMessage(player.PlayerName, avatarRewards, borderRewards, titleRewards, player.MMR, rank, season_data.SeasonName);
            }

            return true;
        }

        public async Task<bool> SendSeasonEndMessage(string username, IList<string> avatars, IList<string> borders, IList<string> titles, int mmrBeforeReset, int rank, string seasonName)
        {
            if (_users.Any(x => x.Value.PlayerName == username))
            {
                var connectionId = _users.Single(x => x.Value.PlayerName == username).Value.ConnectionId;
                if (!_users.ContainsKey(connectionId))
                {
                    return false;
                }
                var user = _users[connectionId];
                await _hub.Clients.Client(connectionId).SendAsync("DisplaySeasonEndMessage", avatars, borders, titles, mmrBeforeReset, rank, seasonName);
            }
            else
            {
                //not logged
                var message = new UserSeasonEndMessage("DisplaySeasonEndMessage", avatars, borders, titles, mmrBeforeReset, rank, seasonName);
                await _databaseService.SaveUserMessage(username, message);
                
            }
            return false;
        }


        public async Task<UserInfo> QueryUserInfo(string username, string password)
        {
            var loginUser = _databaseService.Login(username, password);
            if (loginUser == null) return null;

            // if user is online, attach any pending newly unlocked trinkets
            var onlineUser = _users.Values.FirstOrDefault(x => x.UserName == username);
            if (onlineUser != null && onlineUser.NewlyUnlockedTrinkets != null)
            {
                loginUser.NewlyUnlockedTrinkets = new NewlyUnlockedTrinkets
                {
                    NewAvatars = new List<string>(onlineUser.NewlyUnlockedTrinkets.NewAvatars),
                    NewBorders = new List<string>(onlineUser.NewlyUnlockedTrinkets.NewBorders),
                    NewTitles = new List<string>(onlineUser.NewlyUnlockedTrinkets.NewTitles)
                };
            }
            // give trinkets linked to a counter such as GG
            if (loginUser.GGsReceived >= 100)
            {
                await AddBorder(loginUser.PlayerName, "G_Phoenix");
            }
            if (loginUser.GGsReceived >= 200)
            {
                await AddAvatar(loginUser.PlayerName, "Phoenix");
            }
            if (loginUser.GGsReceived >= 500)
            {
                await AddTitle(loginUser.PlayerName, "GOODGAMER");
            }
            if (loginUser.GamesOver200 >= 1)
            {
                await AddTitle(loginUser.PlayerName, "OCCASIONALDRINKER");
            }
            if (loginUser.GamesOver200 >= 5)
            {
                await AddBorder(loginUser.PlayerName, "G_Beer");
            }
            if (loginUser.GamesOver200 >= 10)
            {
                await AddAvatar(loginUser.PlayerName, "Odrin");
            }
            //
            return loginUser;
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
                user.CurrentAvatar = loginUser.CurrentAvatar;
                user.CurrentBorder = loginUser.CurrentBorder;
                user.CurrentTitle = loginUser.CurrentTitle;
                user.OwnedAvatars = loginUser.OwnedAvatars;
                user.OwnedBorders = loginUser.OwnedBorders;
                user.OwnedTitles = loginUser.OwnedTitles;
                _users.Add(user.ConnectionId, user);

                // give all default avatars
                await AddAvatar(user.PlayerName, "NoAvatar");
                await AddAvatar(user.PlayerName, "GeraltOfRivia"); 
                await AddAvatar(user.PlayerName, "TrissMerigold");
                await AddAvatar(user.PlayerName, "Yennefer");
                // give the seasonal avatars - delete section after season
                // await AddAvatar(user.PlayerName, "ClassicGeralt");
                // give all default borders
                await AddBorder(user.PlayerName, "NoBorder");
                // give all default titles
                await AddTitle(user.PlayerName, "CARDSMITH");
                // give the seasonal titles - delete section after season
                // await AddTitle(user.PlayerName, "PIONEER");

                // if no title, avatar or border is set, set the default ones
                if (user.CurrentBorder == null)
                {
                    await UpdateBorder(user.PlayerName, "NoBorder");
                }
                if (user.CurrentAvatar == null)
                {
                    await UpdateAvatar(user.PlayerName, "NoAvatar");
                }
                if (user.CurrentTitle == null)
                {
                    await UpdateTitle(user.PlayerName, "CARDSMITH");
                }

                // Copy newly unlocked trinkets to the returned UserInfo (map buffer -> DTO)
                if (user.NewlyUnlockedTrinkets != null)
                {
                    loginUser.NewlyUnlockedTrinkets = new NewlyUnlockedTrinkets
                    {
                        NewAvatars = new List<string>(user.NewlyUnlockedTrinkets.NewAvatars),
                        NewBorders = new List<string>(user.NewlyUnlockedTrinkets.NewBorders),
                        NewTitles = new List<string>(user.NewlyUnlockedTrinkets.NewTitles)
                    };
                }

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
                player.CurrentAvatar = user.CurrentAvatar;
                player.CurrentBorder = user.CurrentBorder;
                player.CurrentTitle = user.CurrentTitle;
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
        public async Task<bool> UpdateAvatar(string playername, string AvatarID) // updates the avatar of the user
        {
            _databaseService.UpdateAvatar(playername, AvatarID);
            if (_users.Any(x => x.Value.PlayerName == playername))
            {
                var connectionId = _users.Single(x => x.Value.PlayerName == playername).Value.ConnectionId;
                if (!_users.ContainsKey(connectionId))
                {
                    return false;
                }
                _users[connectionId].CurrentAvatar = AvatarID;
            }
            await Task.CompletedTask;
            return true;
        }

        // adds an avatar to the list of owned avatars of a user
        public async Task<bool> AddAvatar(string playername, string AvatarID)
        {
            var wasAdded = _databaseService.AddAvatar(playername, AvatarID);
            if (wasAdded)
            {
                // Track newly unlocked avatar for online users
                var onlineUser = _users.Values.FirstOrDefault(x => x.PlayerName == playername);
                if (onlineUser != null)
                {
                    onlineUser.NewlyUnlockedTrinkets.NewAvatars.Add(AvatarID);
                }
            }
            await Task.CompletedTask;
            return wasAdded;
        }
        // updates the border of the user
        public async Task<bool> UpdateBorder(string playername, string BorderID)
        {
            _databaseService.UpdateBorder(playername, BorderID);
            if (_users.Any(x => x.Value.PlayerName == playername))
            {
                var connectionId = _users.Single(x => x.Value.PlayerName == playername).Value.ConnectionId;
                if (!_users.ContainsKey(connectionId))
                {
                    return false;
                }
                _users[connectionId].CurrentBorder = BorderID;
            }
            await Task.CompletedTask;
            return true;
        }
        // updates the title of the user
        public async Task<bool> UpdateTitle(string playername, string TitleID)
        {
            _databaseService.UpdateTitle(playername, TitleID);
            if (_users.Any(x => x.Value.PlayerName == playername))
            {
                var connectionId = _users.Single(x => x.Value.PlayerName == playername).Value.ConnectionId;
                if (!_users.ContainsKey(connectionId))
                {
                    return false;
                }
                _users[connectionId].CurrentTitle = TitleID;
            }
            await Task.CompletedTask;
            return true;
        }

        // adds a border to the list of owned borders of a user
        public async Task<bool> AddBorder(string username, string BorderID)
        {
            var wasAdded = _databaseService.AddBorder(username, BorderID);
            if (wasAdded)
            {
                // Track newly unlocked border for online users
                var onlineUser = _users.Values.FirstOrDefault(x => x.PlayerName == username);
                if (onlineUser != null)
                {
                    onlineUser.NewlyUnlockedTrinkets.NewBorders.Add(BorderID);
                }
            }
            await Task.CompletedTask;
            return wasAdded;
        }

        // adds a title to the list of owned titles of a user
        public async Task<bool> AddTitle(string username, string TitleID)
        {
            var wasAdded = _databaseService.AddTitle(username, TitleID);
            if (wasAdded)
            {
                // Track newly unlocked title for online users
                var onlineUser = _users.Values.FirstOrDefault(x => x.PlayerName == username);
                if (onlineUser != null)
                {
                    onlineUser.NewlyUnlockedTrinkets.NewTitles.Add(TitleID);
                }
            }
            await Task.CompletedTask;
            return wasAdded;
        }

        public async Task<bool> SendGG(string MyName, string EnemyName) // send your name to the opponent and trigger GG
        {
            if (_users.Any(x => x.Value.PlayerName == EnemyName))
            {
                var connectionId = _users.Single(x => x.Value.PlayerName == EnemyName).Value.ConnectionId;
                if (!_users.ContainsKey(connectionId))
                {
                    return false;
                }
                var user = _users[connectionId];
                await _hub.Clients.Client(connectionId).SendAsync("DisplayGG", MyName);
                _databaseService.UpdateGGCounter(EnemyName); // update the GG couter
            }
            return false;
        }
        public async Task<bool> SendTaunt(string EnemyName, string TauntID) // 
        {
            if (_users.Any(x => x.Value.PlayerName == EnemyName))
            {

                var connectionId = _users.Single(x => x.Value.PlayerName == EnemyName).Value.ConnectionId;
                if (!_users.ContainsKey(connectionId))
                {
                    return false;
                }
                await _hub.Clients.Client(connectionId).SendAsync("PlayTaunt", TauntID);
                return true;
            }
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
            if (deck?.Leader == "12004" || !(deck.IsBasicDeck() || deck.IsSpecialDeck()))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count >= 1000)
                return false;
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

        public bool SwapDecks(string connectionId, string firstDeckId, string secondDeckId)
        {
            // user must be logged in
            if (!_users.TryGetValue(connectionId, out var user))
                return false;

            // call DB service first (source of truth)
            if (!_databaseService.SwapDecks(user.UserName, firstDeckId, secondDeckId))
                return false;

            // update in-memory decks
            var firstIndex = user.Decks.Select((d, i) => (d, i)).FirstOrDefault(x => x.d.Id == firstDeckId).i;
            var secondIndex = user.Decks.Select((d, i) => (d, i)).FirstOrDefault(x => x.d.Id == secondDeckId).i;

            if (firstIndex < 0 || secondIndex < 0)
                return false;

            (user.Decks[firstIndex], user.Decks[secondIndex]) =
                (user.Decks[secondIndex], user.Decks[firstIndex]);

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
            return "2.1.9";
        }

        public async Task<string> GetNotes(string connectionId)
        {
            await Task.CompletedTask;
            return @"DiyGwent AITest 2.1.9 · 5010 实验服

QQ群：945408322（约战、反馈、DIY 讨论）

AI 对局密码：
ai  杰洛特与希里
ai1 新兵训练
ai2 阿瓦拉克（爆牌）
ai3 奥贝伦王
ai4 铁隼佣兵团
ai5 猎龙人

若可能遇到同密码玩家，在末尾加 #f（也兼容 #）即可强制挑战 AI，例如 ai1#f。

提示：本线路更新较快，可能断线或临时调整；实验数据会独立演进，遇到问题请到群内反馈。";
        }

        public async Task<string> GetNotesEN(string connectionId)
        {
            await Task.CompletedTask;
            return @"DiyGwent AITest 2.1.9 · experimental realm on port 5010

QQ group: 945408322 (matches, feedback, and DIY discussion)

AI match passwords:
ai  Geralt and Ciri
ai1 Recruit Training
ai2 Avallac'h (mill)
ai3 King Auberon
ai4 Iron Falcon
ai5 Dragon Hunter

Append #f (# is also accepted) to force an AI match when another player may use the same password, for example ai1#f.

Note: this realm changes frequently and may be interrupted. Its experimental data evolves independently; report issues in the group.";
        }

        public async Task<string> GetDownloadLink(string connectionId)
        {
            await Task.CompletedTask;
            return string.Empty;
        }

        public async Task<string> GetLatestClientVersion(string connectionId)
        {
            await Task.CompletedTask;
            return @"2.1.9";
        }
        //-------------------------------------------------------------------------
        public int GetUserCount()
        {
            return _users.Count;
        }
        // Display count in game
        public int GetUsersInMatchCount()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return list.Count * 2;
        }
        public int GetUsersInRankedCount()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer && x.Password == "rank").Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return list.Count * 2;
        }
        public int GetUsersvsAICount()
        {
            var ailist = _gwentMatchs.GwentRooms.Where(x => x.IsReady && (x.Player1 is AIPlayer || x.Player2 is AIPlayer)).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return ailist.Count;
        }
        public int GetUsersInCasualCount() // including playing vs friend
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer && x.Password != "rank").Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return list.Count * 2;
        }

        public int GetIsRankQueue()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady == false && x.Password == "rank").Select(x => (x.Player1.PlayerName)).ToList();
            return list.Count();
        }
        public int GetIsCasualQueue()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady == false && x.Password == "").Select(x => (x.Player1.PlayerName)).ToList();
            return list.Count();
        }
        public void InovkeUserChanged()
        {
            OnUserChanged?.Invoke(GetUsers());
        }

        public IList<GameResult> ResultList { get; private set; } = new List<GameResult>();

        public async void MMRTrinkets(string PlayerName, int mymmr) // add trinkets when a certain MMR is reached
        {
            string rank = null;
            string ranktitle = null;
            var seasondata = await _databaseService.QuerySeasonData();

            switch (mymmr)
            {
                case int i when i < 3500:
                    break;
                case int i when i >= 3500 && i < 3650:
                    rank = "Rank3border";
                    ranktitle = "NOVICE";
                    break;
                case int i when i >= 3650 && i < 3800:
                    rank = "Rank6border";
                    ranktitle = "APPRENTICE";
                    break;
                case int i when i >= 3800 && i < 3850:
                    rank = "Rank9border";
                    ranktitle = "JOURNEYMAN";
                    break;
                case int i when i >= 3950 && i < 4100:
                    rank = "Rank12border";
                    ranktitle = "ADEPT";
                    break;
                case int i when i >= 4100 && i < 4250:
                    rank = "Rank15border";
                    ranktitle = "CARDSHARP";
                    break;
                case int i when i >= 4250 && i < 4400:
                    rank = "Rank18border";
                    ranktitle = "MASTER";
                    break;
                case int i when i >= 4400:
                    rank = "Rank21border";
                    ranktitle = "GRANDMASTER";
                    break;
                default:
                    break;
            }

            if (rank != null)
            {
                await AddBorder(PlayerName, rank);
            }
            if (ranktitle != null)
            {
                await AddTitle(PlayerName, ranktitle);
            }

            // In-season rewards driven by SeasonReward configuration
            if (seasondata != null && seasondata.seasonalRewards != null)
            {
                var inSeasonRewards = seasondata.seasonalRewards
                    .Where(r => r.isInSeasonReward && r.minimalMMR > 0)
                    .ToList();

                foreach (var reward in inSeasonRewards)
                {
                    if (mymmr >= reward.minimalMMR)
                    {
                        if (!string.IsNullOrEmpty(reward.avatar))
                        {
                            await AddAvatar(PlayerName, reward.avatar);
                        }
                        if (!string.IsNullOrEmpty(reward.border))
                        {
                            await AddBorder(PlayerName, reward.border);
                        }
                        if (!string.IsNullOrEmpty(reward.title))
                        {
                            await AddTitle(PlayerName, reward.title);
                        }
                    }
                }
            }
        }

        public async void InvokeGameOver(GameResult result, bool isOnlyShow, bool isCountMMR)
        {
            result.isRanked = isCountMMR;
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
                    
                // if a player won a the game with 200+ points in any round, increase User.GamesOver200 by 1
                if (result.RedWinCount == 2 && result.RedScore.Any(x => x >= 200))
                {
                    _databaseService.UpdateGamesOver200(result.RedPlayerName);
                }
                if (result.BlueWinCount == 2 && result.BlueScore.Any(x => x >= 200))
                {
                    _databaseService.UpdateGamesOver200(result.BluePlayerName);
                }
                // update MMR for both players
                _databaseService.UpdateMMR(result.RedPlayerName, Math.Max(RedMMR, 0));
                _databaseService.UpdateMMR(result.BluePlayerName, Math.Max(BlueMMR, 0));

                // update streak for both players

                int GetFactionIndex(string leaderId){
                    Dictionary<Faction, int> factionIndexMap = new Dictionary<Faction, int>
                    {
                        { Faction.Monsters, 0 },
                        { Faction.Nilfgaard, 1 },
                        { Faction.NorthernRealms, 2 },
                        { Faction.ScoiaTael, 3 },
                        { Faction.Skellige, 4 }
                    };

                    var leaderFaction = GwentMap.CardMap[leaderId].Faction;
                    int factionIndex = -1;

                    if (factionIndexMap.ContainsKey(leaderFaction))
                        factionIndex = factionIndexMap[leaderFaction];

                    return factionIndex;                     
                }
               

                _databaseService.UpdateStreak(result.RedPlayerName, result.RedPlayerGameResultStatus == GameStatus.Win ? 0 : (result.RedPlayerGameResultStatus == GameStatus.Lose ? 1 : 2), GetFactionIndex(result.RedLeaderId));
                _databaseService.UpdateStreak(result.BluePlayerName, result.RedPlayerGameResultStatus == GameStatus.Win ? 1 : (result.RedPlayerGameResultStatus == GameStatus.Lose ? 0 : 2), GetFactionIndex(result.BlueLeaderId));

                // add trinkets when a certain MMR is reached
                MMRTrinkets(result.RedPlayerName, RedMMR);
                MMRTrinkets(result.BluePlayerName, BlueMMR);
            }
            else
            {
                // if any player score more than a million points in a casual match, give to both the title "$$$MILLIONAIRE$$$"
                if (result.RedScore.Any(x => x >= 1000000) || result.BlueScore.Any(x => x >= 1000000))
                {
                    await AddTitle(result.RedPlayerName, "$$$MILLIONAIRE$$$");
                    await AddTitle(result.BluePlayerName, "$$$MILLIONAIRE$$$");
                }
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
            int diff = enemyMMR - myMMR;
            if (!isWin)
            {
                diff = Math.Max(diff, -150);
            }
            double e = 1 / (1.0 + Math.Pow(10, diff / 800.0));
            double eta = CalculateEta(s, e, myMMR);
            int newMMR = (int)Math.Round(myMMR + eta * k * (s - e));
            if (isWin && newMMR == myMMR)
            {
                newMMR++;
            }
            return newMMR;
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
        {   // only matches with password containing #w can be spectated
            var list = _gwentMatchs.GwentRooms.Where(x => x.Password.Contains("#w", StringComparison.OrdinalIgnoreCase) && x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName, x.RoomId)).ToList();
            var aiList = _gwentMatchs.GwentRooms.Where(x => x.IsReady && (x.Player1 is AIPlayer || x.Player2 is AIPlayer)).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName, x.RoomId)).ToList();
            return (_users.Select(x => x.Value).Where(x => x.UserState != UserState.Play && x.UserState != UserState.PlayWithAI).GroupBy(x => x.UserState).ToList(), list, aiList);
        }

        public event Action<(IList<IGrouping<UserState, User>>, IList<(string, string)>, IList<(string, string)>)> OnUserChanged;

        public event Action<GameResult> OnGameOver;

        public string GetCardMap()
        {
            return _gwentCardDataService.GetCardMap();
        }
        public string GetAvatarMap()
        {
            return _gwentCardDataService.GetAvatarMap();
        }
        public string GetTitleMap()
        {
            return _gwentCardDataService.GetTitleMap();
        }
        public string GetBorderMap()
        {
            return _gwentCardDataService.GetBorderMap();
        }

        public string GetGameLocales()
        {
            return _gwentLocalizationService.GetGameLocales();
        }

        public int GetPalyernameMMR(string playername) => _databaseService.QueryMMR(playername);

        public Tuple<int,int> GetPalyernameMMRandPeak(string playername) => _databaseService.QueryMMRandPeak(playername);

        public int[] GetPlayernameStreak(string playername) => _databaseService.QueryStreak(playername);

        public async Task<SeasonInfo> GetSeasonData(bool active = true, int id = 0) => await _databaseService.QuerySeasonData(active, id);
        public IList<string> GetUserMessages(string playername) => _databaseService.QueryUserMessages(playername);
        public Task<bool> RemoveUserMessage(string username, int messageId) => _databaseService.RemoveUserMessage(username, messageId);
        public IList<SeasonReward> GetSeasonRewards(int seasonID, string type = "all") => _databaseService.QuerySeasonRewards(seasonID, type);
        public IList<Tuple<string, int>> GetAllMMR(int offset, int limit) => _databaseService.QueryAllMMR(offset, limit);
        public IList<Tuple<string, string, string, string, int, int, IList<int[]>>> GetAllMMRExtended(int offset, int limit) => _databaseService.QueryAllMMRExtended(offset, limit);

        public IList<Tuple<string, int>> GetAllHighestMMR(int offset, int limit) => _databaseService.QueryAllHighestMMR(offset, limit);

        public async Task<bool> ClearNewlyUnlockedTrinkets(string username)
        {
            var onlineUser = _users.Values.FirstOrDefault(x => x.UserName == username);
            onlineUser?.NewlyUnlockedTrinkets.Clear();
            await Task.CompletedTask;
            return true;
        }
    }
}
