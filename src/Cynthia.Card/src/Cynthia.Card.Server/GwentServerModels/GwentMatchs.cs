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
        public async void StartGame(GwentRoom room, bool isSpecial = false, bool isCountMMR = false, string modeId = "", string rulesetVersion = "", string rulesetFingerprint = "")
        {
            GwentServerGame gwentGame = null;
            try
            {
            var featureManifest = _gwentService.GetRuntimeFeatureManifest();
            var activeRuleCardIds = (featureManifest.RuleCards ?? new List<RuleCardDefinition>())
                .Where(x => x != null && x.IsEnabled)
                .Select(x => x.Id)
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal)
                .ToList();
            if (string.IsNullOrWhiteSpace(rulesetVersion))
                rulesetVersion = string.IsNullOrWhiteSpace(featureManifest.RulesetVersion)
                    ? "legacy-custom"
                    : featureManifest.RulesetVersion;
            if (string.IsNullOrWhiteSpace(rulesetFingerprint))
                rulesetFingerprint = CreateCombinedRuleFingerprint(
                    room,
                    featureManifest.RuleCards,
                    rulesetVersion,
                    featureManifest.CardPools,
                    resolveDeckRules: deck => _gwentService.ResolveRuntimeDeckRules(deck));
            if (string.IsNullOrWhiteSpace(modeId))
                modeId = string.IsNullOrWhiteSpace(room.Password) ? "legacy.casual" : "custom.password";
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
            gwentGame = new GwentServerGame(
                player1,
                player2,
                _gwentCardTypeServic,
                result => _gwentService.InvokeGameOver(result, (player1 is AIPlayer || player2 is AIPlayer), isCountMMR),
                isSpecial,
                modeId: modeId,
                rulesetVersion: rulesetVersion,
                rulesetFingerprint: rulesetFingerprint,
                cardMarkerDefinitions: featureManifest.CardMarkerDefinitions,
                resourceDefinitions: featureManifest.ResourceDefinitions,
                activeRuleCardIds: activeRuleCardIds,
                ruleCardDefinitions: featureManifest.RuleCards);
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
            }
            catch (Exception exception)
            {
                Console.Error.WriteLine(
                    $"[ROOM-GAME-FAILURE] room={room?.RoomId} {exception.GetType().Name}: {exception.Message}");
                if (gwentGame != null)
                {
                    try { await gwentGame.AbortDueToResolutionError(exception); }
                    catch (Exception abortError)
                    {
                        Console.Error.WriteLine(
                            $"[ROOM-ABORT-FAILURE] room={room?.RoomId} {abortError.GetType().Name}: {abortError.Message}");
                    }
                }
            }
            finally
            {
                GameEnd(room);
                _gwentService.InovkeUserChanged();
            }
        }

        public bool PlayerJoinMode(ClientPlayer player, GameModeDefinition mode, ResolvedDeckRuleSet rules)
        {
            if (player == null || mode == null || rules == null) return false;
            if (string.Equals(mode.MatchKind, "ai", StringComparison.OrdinalIgnoreCase))
            {
                var ai = CreateAi(mode.AiProfile);
                if (ai == null) return false;
                ApplyAiRuleCards(ai, mode.AiRuleCards);
                var aiRoom = new GwentRoom(player, "mode:" + mode.Id + ":" + rules.Fingerprint);
                aiRoom.AddPlayer(ai);
                GwentRooms.Add(aiRoom);
                // Let StartGame derive the combined fingerprint after the
                // server-authored AI rules have been injected.
                StartGame(aiRoom, false, false, mode.Id, rules.RulesetVersion);
                return true;
            }
            if (!string.Equals(mode.MatchKind, "pvp", StringComparison.OrdinalIgnoreCase)) return false;

            var matchSameRules = !string.Equals(mode.RuleMatchPolicy, "ignore", StringComparison.OrdinalIgnoreCase);
            var key = CreateModeMatchKey(mode, rules);
            foreach (var room in GwentRooms.Where(x => !x.IsReady && x.Password == key).ToList())
            {
                room.AddPlayer(player);
                if (room.IsReady)
                {
                    var featureManifest = _gwentService.GetRuntimeFeatureManifest();
                    var matchFingerprint = matchSameRules
                        ? rules.Fingerprint
                        : CreateCombinedRuleFingerprint(
                            room,
                            featureManifest.RuleCards,
                            rules.RulesetVersion,
                            featureManifest.CardPools,
                            resolveDeckRules: deck => _gwentService.ResolveRuntimeDeckRules(deck));
                    StartGame(
                        room,
                        false,
                        ShouldCountModeMatchAsRanked(mode, room),
                        mode.Id,
                        rules.RulesetVersion,
                        matchFingerprint);
                }
                return true;
            }
            player.CurrentUser.UserState = UserState.Match;
            GwentRooms.Add(new GwentRoom(player, key));
            return true;
        }

        public static string CreateModeMatchKey(GameModeDefinition mode, ResolvedDeckRuleSet rules)
        {
            if (mode == null) throw new ArgumentNullException(nameof(mode));
            if (rules == null) throw new ArgumentNullException(nameof(rules));
            var matchSameRules = !string.Equals(mode.RuleMatchPolicy, "ignore", StringComparison.OrdinalIgnoreCase);
            return "mode:" + mode.Id + (matchSameRules ? ":" + (rules.Fingerprint ?? "") : "");
        }

        public static bool ShouldCountModeMatchAsRanked(GameModeDefinition mode, GwentRoom room)
        {
            if (mode == null || room == null || !mode.IsRanked) return false;
            var hasRuleCards = new[] { room.Player1, room.Player2 }
                .Where(x => x != null)
                .SelectMany(x => x.Deck?.Deck ?? new List<string>())
                .Any(DeckRuleEngine.IsRuleCard);
            return !hasRuleCards || mode.CountRuleMatchesAsRanked;
        }

        public static bool CanJoinExplicitPasswordRoom(GwentRoom room, ClientPlayer player, string password)
        {
            if (room == null || player == null || room.IsReady) return false;
            var requestedPassword = password ?? "";
            var roomPassword = room.Password ?? "";
            var exactPasswordMatch = string.Equals(
                roomPassword,
                requestedPassword,
                StringComparison.OrdinalIgnoreCase);
            var legacyAiFallback = roomPassword.Length == 0 &&
                requestedPassword.StartsWith("ai", StringComparison.OrdinalIgnoreCase);
            return exactPasswordMatch || legacyAiFallback;
        }

        public static string CreateCombinedRuleFingerprint(
            GwentRoom room,
            IEnumerable<RuleCardDefinition> definitions,
            string rulesetVersion,
            IEnumerable<CardPoolDefinition> cardPools = null,
            Func<string, bool> isActiveRuleCard = null,
            Func<DeckModel, ResolvedDeckRuleSet> resolveDeckRules = null)
        {
            if (resolveDeckRules != null)
            {
                var fingerprints = new[] { room.Player1, room.Player2 }
                    .Where(x => x != null)
                    .Select(x => resolveDeckRules(x.Deck)?.Fingerprint ?? "")
                    .ToList();
                return DeckRuleEngine.CombineFingerprints(rulesetVersion, fingerprints);
            }
            isActiveRuleCard = isActiveRuleCard ?? DeckRuleEngine.IsRuleCard;
            var ids = new[] { room.Player1, room.Player2 }
                .Where(x => x != null)
                .SelectMany(x => x.Deck?.Deck ?? new List<string>())
                .Where(isActiveRuleCard)
                .Distinct(StringComparer.Ordinal)
                .OrderBy(x => x, StringComparer.Ordinal)
                .ToList();
            return DeckRuleEngine.Resolve(definitions, ids, rulesetVersion, cardPools).Fingerprint;
        }

        private static AIPlayer CreateAi(string profile)
        {
            switch ((profile ?? "").ToLowerInvariant())
            {
                case "ai0": return new GeraltNovaAI();
                case "ai1": return new SoldierTrainAI();
                case "ai2": return new MillAI();
                case "ai3": return new AuberonKingAI();
                case "ai4": return new IronFalconAI();
                case "ai5": return new ReaverHunterAI();
                default: return null;
            }
        }

        public static void ApplyAiRuleCards(AIPlayer ai, IEnumerable<string> ruleCardIds)
        {
            if (ai?.Deck == null) return;
            if (ai.Deck.Deck == null) ai.Deck.Deck = new List<string>();
            var existing = new HashSet<string>(ai.Deck.Deck, StringComparer.Ordinal);
            foreach (var id in (ruleCardIds ?? Enumerable.Empty<string>())
                .Where(x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.Ordinal))
            {
                if (existing.Add(id)) ai.Deck.Deck.Add(id);
            }
        }

        //以密码的方式进行匹配
        public void PlayerJoin(ClientPlayer player, string password)
        {
            //判断是否是特殊密码
            if (password.ToLower().EndsWith("#f") || password.ToLower().EndsWith("#"))
            {
                switch (password.ToLower().Replace("#f", "").TrimEnd('#').Replace("special", ""))
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
                if (CanJoinExplicitPasswordRoom(room, player, password))
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
                switch (password.ToLower().Replace("#f", "").TrimEnd('#').Replace("special", ""))
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
            if (room == null) return;
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
