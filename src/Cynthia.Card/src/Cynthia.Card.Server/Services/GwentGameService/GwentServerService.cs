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
        public async Task<bool> SendGG(string MyName, string EnemyName) // send your name to the opponent and trigger GG
        {
            if (_users.Any(x => x.Value.UserName == EnemyName))
            {
                var connectionId = _users.Single(x => x.Value.UserName == EnemyName).Value.ConnectionId;
                await _hub.Clients.Client(connectionId).SendAsync("DisplayGG", MyName);
                return false;
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
            if (deck.Leader == "12004")
            {
                return false;
            }
            //添加卡组
            if (!_users.ContainsKey(connectionId))
                return false;
            var user = _users[connectionId];
            if (user.Decks.Count >= 1000)
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
本作永久免费开源,欢迎加群闲聊约战~关注第一消息
群号: 949112936/945408322
查看实时在线人数: http://cynthia.ovyno.com:5005

To dowload the latest version of the game and interact with the community, please visit our Discord at https://discord.gg/Dw9sKgaUZN

#[AI匹配] 输入密码ai/ai1即可挑战ai~
当有其他人匹配时优先玩家匹配,ai密码后缀添加#f(如ai#f)即可强制挑战ai
ai列表：[ai:杰洛特希里]、[ai1：新兵训练]、[ai2：阿瓦拉克]、[ai3：奥贝伦王]、[ai4：铁隼佣兵团]、[ai5：猎龙人]

#注意事项: 
1. 账号密码与原服务器分开，需要重新注册
2. 游戏中有可能断线、更新内容
3. 全部更新内容请参照https://shimo.im/docs/TQdjjwpPwd9hJhK

#diy服更新 20240220
##新卡NEW Card
Skellige:
-被诅咒的卷轴：0-特殊，检视3张不同稀有度的牌，打出一张，丢弃其余两张。
-乌鸦之母：7-德鲁伊，丢弃牌组中的三张铜色特殊牌。己方每丢弃一张特殊牌则生成一只乌鸦。
-维尔卡战士：14-呓语，对相邻单位造成4点伤害。
-鸦母德鲁伊：8-德鲁伊，回合结束时，若同排没有乌鸦则在左侧生成一个乌鸦。
-斯瓦勃洛牧师：10-呓语，受到己方伤害时，对1个单位造成等额伤害。

Scoia'tael:
-卓尔坦的伙伴：0-特殊、谋略，将墓场3张银色/铜色矮人牌返回牌组，随后打出1张铜色矮人牌。位于墓地时，使己方打出的矮人单位获得1点护甲与1点增益。
-巨橡：13-树灵，择一，削弱一个敌军单位一半的基础战力；复活1张铜色树精牌，随后使其返回牌组。
-树精的呵护：0-特殊、自然，打出1张银色/铜色树精牌，使其获得等同于场上友军树精单位数量的增益。
-树人野猪：7-树灵，造成3点伤害，使目标相邻单位移至随机排，若摧毁目标单位则重复1次。
-树精林卫：6-树精，将1个银色/铜色友军单位保留基础战力转化为自身的同名牌，随后使战力低于自身的友军单位获得1点增益。

Northern Realm:
-雷纳德·奥多：10-莱里亚、军官，回合结束时，若至少具有3点增益，则重置自身并使同排友军单位获得1点增益。
-阿德莉亚女王：3-辛特拉，生成1张铜色辛特拉牌，若起始卡组中只有北方领域牌，则将每种各一张铜色辛特拉牌加入牌组。
-维赛基德：4-军官, 辛特拉，造成8点伤害，再造成等同于自身增益的伤害。
-辛特拉老骑士：7-士兵, 辛特拉，己方打出辛特拉单位时，使自身与其获得1点增益。
-辛特拉使者：3-辛特拉，打出1张自身的同名牌，若牌组没有同名牌则将一张同名牌加入牌组。

Monsters:
-残翼：11-野兽，回合结束时，使同排所有敌军单位移至其它排，并对其造成1点伤害。
-奥莉安娜：8-吸血鬼，选择1个敌军单位，使己方场上所有吸血鬼单位各汲取其1点战力
-加尔：4-对两个敌军单位造成5点伤害
-渴血鸟怪：2-吸血鬼，友军吸血鬼单位摧毁单位时召唤一张同名牌。同排友军非同名单位汲取时，汲取相同目标1点战力。
-吸血鬼女：7-吸血鬼，回合结束时，若同排没有战力高于自身的敌军单位，则随机汲取同排1个敌军单位1点战力。

Nilfgaard:
-薇薇恩·塔布里司：6-诅咒生物，将一个友军单位转为起始卡组中的一张金色单位牌,随后使其获得2点增益。
-安娜·亨利叶塔：6-领袖，将牌组中的牌按基础战力从低到高排序，随后打出牌组顶端的一张牌。
-文森特·凡·莫拉汉姆：7-吸血鬼，摧毁1个敌军单位，使其相邻单位各获得其战力一半的增益。
-菲利普·凡·莫拉汉姆：8-吸血鬼，对一个敌军单位造成3点伤害，回合结束时若位于手牌则揭示自身并触发此能力
-莫拉汉姆家斟酒侍者：9-吸血鬼，回合结束时，若对方场上没有金色单位则获得1点增益
-莫拉汉姆家猎手：7-吸血鬼，对一个敌军单位造成3点伤害，若手牌中没有金色牌则重复1次。

Neutral:
-爱丽丝:庄园幽影：13-诅咒生物，休战，向双方手牌中添加两张爱丽丝的伙伴。
-阿扎·贾维德：9-法师，生成2张“圣甲虫”。
-波维斯的科恩：7-猎魔人，造成5点伤害，若摧毁目标单位则降低1点伤害并重复此能力。
-哈吉的伊斯贝尔：9-法师，重置1个单位，若为友军单位则重复1次。
-穿甲弹：0-特殊、道具，对一个敌军单位造成8点伤害（无视护甲），若其具有增益则改为10点。
-紫藤花：0-特殊、炼金选择2个单位，若战力为偶数使其获得6点增益，若战力为奇数对其造成6点伤害。

##改动 Alterations
重做性改动
-恩约夫：11-摧毁双方场上所有战力低于3的单位
-暴风雨：0-选择4排降下倾盆大雨，若其上有倾盆大雨则改为降下史凯利杰风暴。
-克努特：4-打出牌组中基础战力最高的铜色单位牌，对其造成等同其战力一半的伤害。
-图尔赛克家族战船：9-回合结束时，随机对1个未受伤的单位造成2点伤害。
-防盾：10-右侧相邻单位受到伤害时，优先抵扣自身的护甲。驱动：获得6点护甲
-辛特拉皇家护卫：8-使自身获得3点增益，若己方落后则改为使相邻单位获得3点增益。
-破坏者：1-随机打出1张铜色道具牌，使其造成的伤害提升1点
-莫拉汉姆家仆从：8-免疫，被隐匿时获得5点增益。
-月之尘：0-造成5点伤害，随后随机造成1点伤害4次。

平衡性改动
-林语者：战力6>7
-克尔图里斯：摧毁5以下>摧毁6以下
-米薇：战力7>8，使场上、手牌、牌组中的各一个单位获得4点增益
-不朽者骑兵：增益2>增益3
-鬼针草煎药、合欢草煎药：去除炼金标签
-希拉德：去除力竭，增加佚亡
-路途埋伏：去除力竭
-凯瑞丝：无所畏惧：位于墓地时也会触发效果
-雷蒂亚：造成手牌中中立牌数量的伤害>造成2次手牌中中立牌数量的伤害
-原始野性：造成3点伤害>造成4点伤害，强化2

还原性改动
- 亚尔潘·齐格林、马哈坎捍卫者、利维亚的杰洛特、暗影长者、暗算、迪门家族走私贩、特莉丝·梅莉葛德


diy服20231229更新
## 新增卡牌
- 巴纳巴斯·贝肯鲍尔：6-使1个其它友军单位获得2点增益，墓地中每有1种铜色道具牌便重复1次。
- 考德威尔伯爵：10-交换2个敌军单位的基础战力。
- 孤独的勇士：9-回合结束时，若场上没有其它友军单位则获得4点增益。
- 斯瓦勃洛：9-对牌组中的所有单位牌造成2点伤害，随后使其获得2点强化。
- 欧特克尔：8-使对方场上的“倾盆大雨”伤害提升1点，若自身受伤则额外提升1点。
- 斯瓦勃洛争斗者：7-对1个敌军单位造成4点伤害，若位于灾厄下则改为造成8点伤害。
- 防盾：10-若牌组没有同名牌则添加一张，操控：获得4点护甲。
- 辛特拉皇家护卫：8-己方总点数落后时，使同名牌获得3点增益。
- 不朽者骑兵：11-锁定自身，回合结束时使1个随机友军单位获得2点增益。
- 物竞天择：0-生成2个“蟹蜘蛛幼虫”，墓地中每有1张同名牌额外生成1个。
- 蝠翼脑魔：5-随机对敌军单位造成1点伤害5次，若目标受伤则改为汲取。
- 林地徘徊者：9-对1个敌军单位造成1点削弱，若目标位于“蔽日浓雾”下则改为3点。
- 树精族母：10-随机使牌组中战力最低的单位牌获得2点增益，若为树精则改为2点强化。
- 林语者：6-对双方同排所有非树精单位造成2点伤害。
- 战前准备：0-从手牌打出一张铜色士兵牌并使其获得2点增益，随后抽一张牌。
- 月之尘炸弹：0-摧毁1个战力不高于5的敌军单位，己方打出谋略牌时，复活并放逐1张同名牌。
- 莫拉汉姆家仆从：8-对方手牌中每有1张金色牌则获得2点增益。

## 修改卡牌
- 贝克尔的黑暗之镜：最多133点伤害>最多11点伤害
- 活体盔甲：金>银，战力13>战力11
- 图尔赛克战船：重做为：回合结束时，随机对1个未受伤的敌军单位造成1点伤害。
- 尖啸女海妖：战力1>战力2
- 海之歌者：删除，替换为欧特克尔
- 原始野性：随机打出>打出
- 大使：14增益>12增益
- 罗契：冷酷之心：重做为：择一：打出1张低于自身战力的银色/铜色泰莫利亚单位牌；摧毁1个背面向上的伏击敌军单位。
- 莱里亚镰刀手：使牌组1个单位获得4点增益>使牌组1个铜色单位获得3点增益
- 法芙：重做为：己方打出领袖牌时，召唤自身。回合结束时，使战力与自身相同的友军单位获得1点增益。
- 长弓树精：选择时显示基础战力低于自身的单位。
- 维里赫德旅破坏者：重做为：随机打出1张铜色道具牌，若牌组数量低于自身战力，则改为复活1张铜色道具牌。
- 齐齐魔工兵：重做为：使同排其它友军类虫生物单位获得2点增益。
- 加斯科：铁隼之首：删除保持手牌数相同效果，增加部署效果：魅惑最强的敌军单位。

diy服2023年4月14日调整
# 增强
大赦：使1个非间谍铜色/银色敌军单位返回对方手牌 -> 使1个铜色/银色敌军单位返回对方手牌，并将其战力设为1

# 削弱：
海玫家族诗人：buff 2 -> buff 1
克尔图里斯：低于6 -> 低于5
德拉蒙突袭者：不再强化场上的单位



diy服2023年3月13日调整
增强：
杰洛特：15->17，（ai1降低新星比例）
操作者：5->7，
法兰：7->9，
弗妮希尔突击队： 6->8
希里：6->9,无护甲,
特莉丝：打5->7，
贝克尔的黑暗之镜：不超过10->13点，
米薇：增益4->5，
军旗手：己方除了召唤之外的所有打出士兵都会触发
神灯：位于墓地时最后的愿望多1个选项，

新卡：
融雪 中立有机：随机使1个友军单位获得2点增益。重复一次。本回合中每打出过1张牌便额外重复1次。

bug怪兽大间谍计时没归零
bug攻城大师触发被锁的机械单位
bug黑暗之境会在结算伤害之前选择增益单位
bug家族诗人没法选金银单位

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
        // Display count in game
        public int GetUsersInMatchCount()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return list.Count*2;
        }
        public int GetUsersInRankedCount()
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer && x.Password == "rank" ).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return list.Count*2;
        }
        public int GetUsersvsAICount()
        {
            var ailist = _gwentMatchs.GwentRooms.Where(x => x.IsReady && (x.Player1 is AIPlayer || x.Player2 is AIPlayer)).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return ailist.Count;
        }
        public int GetUsersInCasualCount() // including playing vs friend
        {
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady && x.Player1 is ClientPlayer && x.Player2 is ClientPlayer && x.Password != "rank" ).Select(x => (x.Player1.PlayerName, x.Player2.PlayerName)).ToList();
            return list.Count*2;
        }
        
        public int GetIsRankQueue()
        {            
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady == false && x.Password == "rank" ).Select(x => (x.Player1.PlayerName)).ToList();
            return list.Count();
        }
        public int GetIsCasualQueue()
        {            
            var list = _gwentMatchs.GwentRooms.Where(x => x.IsReady == false && x.Password == "" ).Select(x => (x.Player1.PlayerName)).ToList();
            return list.Count();
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

        public IList<Tuple<string, int>> GetAllHighestMMR(int offset, int limit) => _databaseService.QueryAllHighestMMR(offset, limit);
    }
}
