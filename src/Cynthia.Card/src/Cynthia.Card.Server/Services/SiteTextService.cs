using System;
using System.Collections.Generic;
using System.Globalization;

namespace Cynthia.Card.Server.Services
{
    public class SiteTextService
    {
        private static readonly IReadOnlyDictionary<string, (string Chinese, string English)> Texts =
            new Dictionary<string, (string Chinese, string English)>(StringComparer.Ordinal)
            {
                ["SiteTitle"] = ("昆特小窝 · AITest", "Gwent Workshop · AITest"),
                ["SiteDescription"] = ("由玩家与 AI 共同维护的昆特牌实验服。", "A Gwent test realm maintained by players and AI."),
                ["ExperimentalChannel"] = ("实验频道", "Experimental channel"),
                ["Navigation"] = ("主导航", "Main navigation"),
                ["SkipToContent"] = ("跳到正文", "Skip to content"),
                ["OpenNavigation"] = ("打开或收起导航", "Open or close navigation"),
                ["Overview"] = ("概览", "Overview"),
                ["Competition"] = ("竞技", "Competition"),
                ["Workshop"] = ("工坊", "Workshop"),
                ["Community"] = ("社区", "Community"),
                ["Home"] = ("主页", "Home"),
                ["Results"] = ("对战记录", "Match results"),
                ["Mmr"] = ("天梯积分", "Ladder rating"),
                ["TopBoard"] = ("天梯荣誉榜", "Hall of fame"),
                ["Seasons"] = ("赛季", "Seasons"),
                ["DiyDesign"] = ("DIY 设计", "DIY design"),
                ["Vote"] = ("吉安所 · 卡牌审核", "Card Review"),
                ["CardArt"] = ("达盖尔的旗帜", "Dagur's Banner"),
                ["Counter"] = ("点点点", "Counter"),
                ["SpecialThanks"] = ("特别感谢", "Special thanks"),
                ["Sponsors"] = ("投食名单", "Supporters"),
                ["MaintainedNotice"] = ("持续试验，随时迭代", "Always experimenting, always iterating"),
                ["ServiceOnline"] = ("AITest 服务在线", "AITest service online"),
                ["Language"] = ("语言", "Language"),
                ["LoginFailed"] = ("登录失败，请检查账号或密码。", "Sign-in failed. Check your account and password."),
                ["LoggingIn"] = ("正在确认登录…", "Signing in…"),
                ["Account"] = ("账号", "Account"),
                ["Password"] = ("密码", "Password"),
                ["Login"] = ("登录", "Sign in"),
                ["WelcomeUser"] = ("欢迎回来，{0}", "Welcome back, {0}"),
                ["Logout"] = ("登出", "Sign out"),
                ["HeroEyebrow"] = ("AITest · 玩家实验场", "AITest · A playground for players"),
                ["HeroTitle"] = ("把昆特，重新放回玩家手里。", "Put Gwent back in players' hands."),
                ["HeroDescription"] = ("一个由社区创意与 AI 协作驱动的长期实验分支。这里允许大胆改动，也认真对待每一次对局。", "A long-running experimental branch shaped by community ideas and AI-assisted development. Bold changes are welcome; every match still matters."),
                ["JoinGroup"] = ("加入 QQ 群", "Join the QQ group"),
                ["CurrentChannel"] = ("当前测试入口", "Current test endpoint"),
                ["StableDiyChannel"] = ("DIY 稳定测试服", "Stable DIY realm"),
                ["OfficialChannel"] = ("正式服端口", "Official server port"),
                ["LiveStatus"] = ("实时玩家状态", "Live player status"),
                ["CurrentlyOnline"] = ("目前在线", "Currently online"),
                ["PlayersUnit"] = ("人", "players"),
                ["NoPlayers"] = ("暂时没有玩家在线。也许你会是这一局的第一位。", "No one is online yet. You could be the first player in the next match."),
                ["OnlineNow"] = ("正在牌桌附近", "Around the tables"),
                ["ServerGuide"] = ("服务器说明", "Server guide"),
                ["ServerGuideBody"] = ("本站是 AITest 测试服，当前客户端连接 5010；DIY 稳定测试服使用 5005；正式服使用 5000。不同线路请使用对应客户端或端口。", "This site is the AITest realm on port 5010. The stable DIY realm uses 5005, while the official server uses 5000. Use the matching client or endpoint for each realm."),
                ["GroupTitle"] = ("一起约牌，也一起造牌", "Find a match. Help shape the game."),
                ["GroupBody"] = ("QQ群 945408322，欢迎来闲聊、约牌、反馈问题或分享 DIY 灵感。", "QQ group 945408322 is open for chat, matches, bug reports, and DIY card ideas."),
                ["QuickStart"] = ("从这里开始", "Start here"),
                ["QuickDiyTitle"] = ("浏览 DIY 卡牌", "Browse DIY cards"),
                ["QuickDiyBody"] = ("查看玩家创作与当前实验内容。", "Explore player-made cards and current experiments."),
                ["QuickRankTitle"] = ("查看天梯", "View the ladder"),
                ["QuickRankBody"] = ("了解当前积分与荣誉榜。", "See current ratings and the hall of fame."),
                ["QuickResultsTitle"] = ("回顾对局", "Review matches"),
                ["QuickResultsBody"] = ("查询最近的牌局与结果。", "Browse recent games and their results."),
                ["Enter"] = ("进入", "Open"),
                ["LocalPreview"] = ("AITest 本地预览", "AITest local preview"),
                ["DataArchive"] = ("牌桌档案", "Table archive"),
                ["RecentMatchesTitle"] = ("最近对局", "Recent matches"),
                ["RecentMatchesDescription"] = ("查看 AITest 最近完成的对局、领袖与逐回合比分。页面最多保留最新 50 场。", "Review the latest completed AITest matches, leaders, and round-by-round scores. Up to 50 recent games are shown."),
                ["MatchesShown"] = ("场记录", "matches shown"),
                ["LatestMatchLimit"] = ("最多 50 场", "Latest 50"),
                ["RankedMatch"] = ("排位", "Ranked"),
                ["CasualMatch"] = ("休闲", "Casual"),
                ["FirstMove"] = ("先手", "First move"),
                ["SecondMove"] = ("后手", "Second move"),
                ["UnknownPlayer"] = ("未知玩家", "Unknown player"),
                ["UnknownLeader"] = ("未知领袖", "Unknown leader"),
                ["Win"] = ("胜", "Win"),
                ["Loss"] = ("负", "Loss"),
                ["Draw"] = ("平", "Draw"),
                ["RoundShort"] = ("回合", "Round"),
                ["NoMatchesTitle"] = ("还没有可展示的对局", "No matches to show yet"),
                ["NoMatchesBody"] = ("完成一场 AITest 对局后，最新结果会出现在这里。", "The latest result will appear here after an AITest match is completed."),
                ["LadderArchive"] = ("竞技档案", "Competitive archive"),
                ["CurrentLadderTitle"] = ("当前天梯积分", "Current ladder rating"),
                ["CurrentLadderDescription"] = ("按玩家当前 MMR 排序，实时反映本赛季的竞技位置。", "Players ordered by current MMR, reflecting the live competitive standings for this season."),
                ["HighestLadderTitle"] = ("历史最高积分", "All-time peak rating"),
                ["HighestLadderDescription"] = ("记录每位玩家曾经达到的最高 MMR，保留那些值得纪念的高峰。", "Each player's highest recorded MMR, preserving the peaks worth remembering."),
                ["RankedPlayers"] = ("位玩家", "ranked players"),
                ["TopRankLimit"] = ("前 500 名", "Top 500"),
                ["RankColumn"] = ("排名", "Rank"),
                ["PlayerColumn"] = ("玩家", "Player"),
                ["CurrentRatingColumn"] = ("当前 MMR", "Current MMR"),
                ["PeakRatingColumn"] = ("最高 MMR", "Peak MMR"),
                ["NoRankingsTitle"] = ("榜单暂时为空", "The ladder is empty"),
                ["NoRankingsBody"] = ("有玩家完成排位对局后，这里会自动出现积分记录。", "Ratings will appear automatically after players complete ranked matches."),
                ["DataUnavailableTitle"] = ("数据暂时不可用", "Data is temporarily unavailable"),
                ["DataUnavailableBody"] = ("本次页面没有成功读取数据，其他导航仍可继续使用。稍后刷新即可重试。", "This page could not load its data. Navigation remains available; refresh later to try again."),
                ["ArtArchive"] = ("美术档案", "Art archive"),
                ["CardArtTitle"] = ("达盖尔的旗帜", "Dagur's Banner"),
                ["CardArtDescription"] = ("浏览服务器收录的卡面网页预览，图片按需加载，并可通过 Art ID 快速筛选。", "Browse the server's web artwork previews. Images load on demand and can be filtered by Art ID."),
                ["ArtAvailable"] = ("张网页预览", "web previews"),
                ["CardArtReadinessNote"] = ("网页预览不等于客户端可直接使用：正式制卡前仍需核对 Unity 全尺寸卡面、CardMap 定义与牌组缩略图是否齐全。", "A web preview is not automatically client-ready. Before using an Art ID, verify its Unity full-size artwork, CardMap definition, and deck-list miniature."),
                ["SearchArt"] = ("搜索 Art ID", "Search Art ID"),
                ["NoArtTitle"] = ("没有找到卡面", "No artwork found"),
                ["NoArtBody"] = ("换一个 Art ID，或清空搜索条件再试。", "Try another Art ID or clear the search field."),
                ["PreviousPage"] = ("上一页", "Previous"),
                ["NextPage"] = ("下一页", "Next"),
                ["PageLabel"] = ("页", "Page"),
                ["CardArtPreview"] = ("卡面", "Card artwork"),
                ["SeasonArchive"] = ("赛季档案", "Season archive"),
                ["SeasonsTitle"] = ("赛季与环境", "Seasons and the field"),
                ["SeasonsDescription"] = ("按赛季回顾排位对局、每日活跃度、阵营选择与奖励。统计结果会短暂缓存，避免每次打开页面都重新扫描历史对局。", "Review ranked matches, daily activity, faction choices, and rewards by season. Statistics are briefly cached so opening the page does not rescan match history every time."),
                ["SeasonCount"] = ("个赛季", "seasons"),
                ["ActiveSeason"] = ("进行中", "Active"),
                ["CompletedSeason"] = ("已结束", "Completed"),
                ["UpcomingSeason"] = ("即将开始", "Upcoming"),
                ["RankedMatches"] = ("排位对局", "Ranked matches"),
                ["PlayerPicks"] = ("阵营选择", "Faction picks"),
                ["ActiveDays"] = ("有对局天数", "Active days"),
                ["DailyActivity"] = ("每日活跃度", "Daily activity"),
                ["RecentActivityWindow"] = ("最近 32 个有记录的日期", "Latest 32 recorded days"),
                ["FactionBreakdown"] = ("阵营表现", "Faction performance"),
                ["FactionColumn"] = ("阵营", "Faction"),
                ["PicksColumn"] = ("选择次数", "Picks"),
                ["WinsColumn"] = ("胜", "Wins"),
                ["DrawsColumn"] = ("平", "Draws"),
                ["LossesColumn"] = ("负", "Losses"),
                ["WinRateColumn"] = ("胜率", "Win rate"),
                ["SeasonRewards"] = ("赛季奖励", "Season rewards"),
                ["RewardsGranted"] = ("奖励已发放", "Rewards granted"),
                ["NoRewards"] = ("本赛季没有公开的奖励配置。", "No public reward configuration is available for this season."),
                ["PositionRequirement"] = ("排名", "Position"),
                ["MmrRequirement"] = ("MMR", "MMR"),
                ["NoSeasonsTitle"] = ("还没有赛季档案", "No season archive yet"),
                ["NoSeasonsBody"] = ("创建赛季并产生排位对局后，统计会出现在这里。", "Statistics will appear after a season is created and ranked matches are played."),
                ["NorthernRealms"] = ("北方领域", "Northern Realms"),
                ["Nilfgaard"] = ("尼弗迦德", "Nilfgaard"),
                ["Skellige"] = ("史凯利格", "Skellige"),
                ["ScoiaTael"] = ("松鼠党", "Scoia'tael"),
                ["Monsters"] = ("怪兽", "Monsters")
            };

        public bool IsEnglish => IsEnglishCulture(CultureInfo.CurrentUICulture);

        public static bool IsEnglishCulture(CultureInfo culture)
        {
            return string.Equals(
                culture?.Name,
                "en-US",
                StringComparison.OrdinalIgnoreCase);
        }

        public string this[string key]
        {
            get
            {
                if (!Texts.TryGetValue(key, out var value))
                {
                    return key;
                }

                return IsEnglish ? value.English : value.Chinese;
            }
        }

        public string Format(string key, params object[] arguments)
        {
            return string.Format(CultureInfo.CurrentUICulture, this[key], arguments);
        }
    }
}
