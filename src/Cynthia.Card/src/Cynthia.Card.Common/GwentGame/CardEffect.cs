using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    public abstract class CardEffect : Effect, IHasEffects
    {
        public GameCard Card { get; set; }//宿主
        public IGwentServerGame Game { get; set; }//游戏本体
        public int AnotherPlayer { get => Game.AnotherPlayer(Card.PlayerIndex); }
        public int PlayerIndex { get => Card.PlayerIndex; }
        public int Countdown { get => Card.Status.Countdown; }
        public EffectSet Effects { get; }
        public async Task SetCountdown(int? value = default, int? offset = default)
        {
            Card.Status.Countdown = (value ?? Card.Status.Countdown) + (offset ?? 0);
            await Game.ShowCardNumberChange(Card, Card.Status.Countdown, NumberType.Countdown);
            if (Card.Status.Countdown == 0)
                Card.Status.IsCountdown = false;
            else
                Card.Status.IsCountdown = true;
            await Game.ShowSetCard(Card);
        }
        public CardEffect(IGwentServerGame game, GameCard card)
        {
            Game = game;
            Card = card;
            Effects = new EffectSet(this);
            Effects.Add(this);
        }
        public virtual async Task CardUse()//使用
        {
            var count = 0;
            await CardUseStart();
            //历史卡牌
            Game.HistoryList.Add((Card.PlayerIndex, Card.Status.CardId));
            if (Card.Status.CardRow.IsOnStay())
                count = await CardUseEffect();
            if (Card.Status.CardRow.IsOnStay())
                await CardUseEnd();
            await PlayStayCard(count, false);
        }
        public virtual async Task Play(CardLocation location)//放置
        {
            var isSpying = await CardPlayStart(location);
            //历史卡牌
            Game.HistoryList.Add((isSpying ? AnotherPlayer : Card.PlayerIndex, Card.Status.CardId));
            var count = 0;
            if (Card.Status.CardRow.IsOnPlace())
                count = await CardPlayEffect(isSpying);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown(isSpying);
            //await Game.Debug($"放置了卡牌:{Card.Status.CardInfo().Name}");
            await PlayStayCard(count, isSpying);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDownEffect(isSpying);
        }
        //-----------------------------------------------------------
        //公共效果
        public virtual async Task ToCemetery(CardBreakEffectType type = CardBreakEffectType.ToCemetery)//进入墓地触发
        {
            // await Game.Debug($"移动到墓地!将卡牌{Card.CardInfo().Name}");
            var isDead = Card.Status.CardRow.IsOnPlace();
            var deadposition = Game.GetCardLocation(Card);
            if (type != CardBreakEffectType.ToCemetery)//直接移动到墓地,动画
                await Game.ShowCardBreakEffect(Card, type);
            Repair();
            if (type == CardBreakEffectType.ToCemetery)
            {
                if (Card.Status.CardRow.IsOnPlace())
                {
                    await Game.ShowCardOn(Card);
                    await Game.ClientDelay(200);
                    await Game.ShowSetCard(Card);
                    await Game.ClientDelay(200);
                    if (Card.Status.Strength <= 0)
                    {
                        await Banish();
                        return;
                    }
                }
                await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyCemetery, CardIndex = 0 }, Card);
                await Game.ClientDelay(400);
            }
            else
            {
                var row = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow);
                var taget = Game.RowToList(Card.PlayerIndex, RowPosition.MyCemetery);
                await Game.LogicCardMove(Card, taget, 0);
            }
            if (Card.Status.IsDoomed)//如果是佚亡,放逐
            {
                await Banish();
                return;
            }
            if (Card.Status.CardRow != RowPosition.Banish)
                await Game.SendEvent(new AfterCardToCemetery(Card, deadposition));//.OnCardToCemetery(Card, deadposition);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //进入墓地(遗愿),应该触发对应事件<暂未定义,待补充>
            if (isDead && Card.Status.CardRow != RowPosition.Banish)//如果从场上进入墓地,并且没有被放逐
                await Game.SendEvent(new AfterCardDeath(Card, deadposition));//await Game.OnCardDeath(Card, deadposition);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            await Game.SetPointInfo();
            await Game.SetCountInfo();
        }
        public virtual void Repair(bool isLockReset = false)
        {
            if (isLockReset)
                Card.Status.IsLock = false;
            Card.Status.Armor = 0; //护甲归零
            Card.Status.HealthStatus = 0;//没有增益和受伤
            Card.Status.IsCardBack = false; //没有背面
            Card.Status.IsResilience = false;//没有坚韧
            Card.Status.IsShield = false; //没有昆恩
            Card.Status.IsSpying = false; //没有间谍
            Card.Status.Conceal = false;  //没有隐藏
            Card.Status.IsReveal = false; //没有揭示
        }
        public virtual async Task Banish()//放逐
        {
            //需要补充
            if (Card.Status.CardRow.IsOnRow())
            {
                await Game.ShowCardBreakEffect(Card, CardBreakEffectType.Banish);
            }
            var list = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow);
            list.RemoveAt(list.IndexOf(Card));
            //所在排为放逐
            Card.Status.CardRow = RowPosition.Banish;
            await Game.SetCountInfo();
            await Game.SetPointInfo();
            await Game.SetCemeteryInfo(Card.PlayerIndex);
        }
        public virtual async Task RoundEnd()
        {   //当回合结束的时候,如果在场上,进行处理
            if (!Card.Status.CardRow.IsOnPlace()) return;

            if (Card.Status.IsResilience)
            {
                Card.Status.Armor = 0; //护甲归零
                if (Card.Status.HealthStatus >= 0)//没有受伤
                    Card.Status.HealthStatus = 0;
                await Card.Effect.Resilience();
                return;
            }
            Repair();
            if (Card.Status.Strength > 0)
            {
                await Game.ShowCardOn(Card);
                await Game.ShowSetCard(Card);
                await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyCemetery, CardIndex = 0 }, Card);
            }
            else
            {
                await Game.ShowCardBreakEffect(Card, CardBreakEffectType.Banish);
                var row = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow);
                var taget = Game.RowToList(Card.PlayerIndex, RowPosition.MyCemetery);
                await Game.LogicCardMove(Card, taget, 0);
            }
            if (Card.Status.IsDoomed)//如果是佚亡,放逐
            {
                await Banish();
                return;
            }
        }
        //-----------------------------------------------------------
        //特殊卡的单卡使用
        public virtual async Task CardUseStart()//使用前移动
        {
            Card.Status.IsReveal = false;//不管怎么样,都先设置成非揭示状态
            if (!Card.Status.CardRow.IsOnStay())
                await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            await Game.ClientDelay(200);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //打出了特殊牌,应该触发对应事件<暂未定义,待补充>
            //await Game.OnSpecialPlay(Card);
            await Game.SendEvent(new BeforeSpecialPlay(Card));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task CardUseEnd()//使用结束
        {
            await Game.ClientDelay(300);
            await ToCemetery();
        }
        //----------------------------------------------------------------------------
        //单位卡的单卡放置
        public virtual async Task<bool> CardPlayStart(CardLocation location)//先是移动到目标地点
        {
            var isSpying = !location.RowPosition.IsMyRow();
            Card.Status.IsReveal = false;//不管怎么样,都先设置成非揭示状态
            await Game.ShowCardOn(Card);
            await Game.ShowCardMove(location, Card);
            await Game.ClientDelay(400);
            //await Game.Debug("开始群发,一段部署前事件");
            // await Game.OnUnitPlay(Card);
            await Game.SendEvent(new AfterUnitPlay(Card));
            //await Game.Debug("群发完毕");
            return isSpying;//有没有间谍呢
        }
        public virtual async Task PlayStayCard(int count, bool isSpying)
        {
            var stayPlayer = isSpying ? AnotherPlayer : Card.PlayerIndex;
            for (var i = 0; i < count; i++)
            {
                if (Game.PlayersStay[stayPlayer][0].CardInfo().CardType == CardType.Special)
                {
                    await Game.PlayersStay[stayPlayer][0].Effect.CardUse();
                }
                else
                {
                    var location = await Game.GetPlayCard(Game.PlayersStay[stayPlayer][0]);
                    if (location.RowPosition.IsInCemetery())
                        await Game.PlayersStay[stayPlayer][0].Effect.ToCemetery();
                    else
                        await Game.PlayersStay[stayPlayer][0].Effect.Play(location);
                }
            }
        }
        public virtual async Task CardDown(bool isSpying)
        {
            await Game.ShowCardDown(Card);
            await Game.SetPointInfo();
            if (isSpying)
                await Spying();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //打出了卡牌,应该触发对应事件<暂未定义,待补充>
            // await Game.OnUnitDown(Card);
            await Game.SendEvent(new AfterUnitDown(Card));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //-----------------------------------------
            //大概,判断天气陷阱一类的(血月坑陷)(已经交给游戏事件处理)
        }
        //=====================================================================================================
        //单位卡的单卡所受效果
        public virtual async Task Strengthen(int num, GameCard source = null)//强化
        {
            if (num <= 0) return;
            if (source != null)
            {
                await Game.ShowBullet(source, Card, BulletType.GreenLight);
            }
            Card.Status.Strength += num;
            await Game.ShowCardNumberChange(Card, num, NumberType.White);
            //await Game.ClientDelay(50);
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //await Game.ClientDelay(150);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //强化,应该触发对应事件<暂未定义,待补充>
            //await Game.OnCardStrengthen(Card, num, source);
            await Game.SendEvent(new AfterCardStrengthen(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Weaken(int num, GameCard source = null)//削弱
        {
            if (num <= 0 || Card.Status.CardRow == RowPosition.Banish) return;
            //此为最大承受值
            var bear = Card.Status.Strength;
            if (num > bear) num = bear;
            if (source != null)
            {
                await Game.ShowBullet(source, Card, BulletType.RedLight);
            }
            //最大显示的数字,不超过这个值
            var showBear = Card.Status.Strength + Card.Status.HealthStatus;
            Card.Status.Strength -= num;
            await Game.ShowCardNumberChange(Card, num > showBear ? -showBear : -num, NumberType.White);
            //await Game.ClientDelay(50);
            if (Card.Status.Strength < -Card.Status.HealthStatus) Card.Status.HealthStatus = -Card.Status.Strength;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //await Game.ClientDelay(150);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //有单位被削弱了,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardWeaken(Card, num, source);
            await Game.SendEvent(new AfterCardWeaken(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            if ((Card.Status.Strength + Card.Status.HealthStatus) <= 0)
            {
                if (Card.Status.Strength > 0)
                {
                    await ToCemetery();
                }
                else
                {
                    await Banish();
                }
            }
        }
        public virtual async Task Boost(int num, GameCard source = null)//增益
        {
            if (num <= 0 || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            if (source != null)
            {
                await Game.ShowBullet(source, Card, BulletType.GreenLight);
            }
            Card.Status.HealthStatus += num;
            await Game.ShowCardNumberChange(Card, num, NumberType.Normal);
            //await Game.ClientDelay(50);
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //await Game.ClientDelay(150);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //有卡牌增益,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardBoost(Card, num, source);
            await Game.SendEvent(new AfterCardBoost(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Damage(int num, GameCard source = null, BulletType type = BulletType.Arrow, bool isPenetrate = false)//伤害
        {
            if (num <= 0 || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            //最高承受伤害,如果穿透的话,不考虑护甲
            var die = false;
            var isArmor = Card.Status.Armor > 0;
            var bear = (Card.Status.Strength + (isPenetrate ? 0 : Card.Status.Armor) + Card.Status.HealthStatus);
            if (num >= bear)
            {
                num = bear;//如果数值大于最高伤害的话,进行限制
                die = true;//死亡,不会触发任何效果
            }
            if (source != null)
            {
                await Game.ShowBullet(source, Card, type);
            }
            //--------------------------------------------------------------
            //护甲处理
            if (Card.Status.Armor > 0 && !isPenetrate)//如果有护甲并且并非穿透伤害
            {
                //首先播放破甲动画
                await Game.ShowCardIconEffect(Card, CardIconEffectType.BreakArmor);
                if (Card.Status.Armor >= num)//如果护甲更高的话
                {
                    Card.Status.Armor -= num;
                    //8888888888888888888888888888888888888888888888888888888888888888888888
                    //护甲值降低,应该触发对应事件<暂未定义,待补充>
                    // await Game.OnCardSubArmor(Card, num, source);
                    await Game.SendEvent(new AfterCardSubArmor(Card, num, source));
                    //8888888888888888888888888888888888888888888888888888888888888888888888
                    num = 0;
                    await Game.ShowSetCard(Card);//更新客户端的护甲值
                    return;
                }
                else//如果伤害更高
                {
                    num -= Card.Status.Armor;
                    Card.Status.Armor = 0;
                }
                await Game.ShowSetCard(Card);//更新客户端的护甲值
            }
            //-------------------------------------------------------------
            //战力值处理
            var isHurt = num > 0;
            if (num > 0)
            {
                Card.Status.HealthStatus -= num;
                await Game.ShowCardNumberChange(Card, -num, NumberType.Normal);
                //await Game.ClientDelay(50);
                await Game.ShowSetCard(Card);
                await Game.SetPointInfo();
                //await Game.ClientDelay(150);
                if ((Card.Status.HealthStatus + Card.Status.Strength) <= 0)
                {
                    await ToCemetery();
                    return;
                }
            }
            if (Card.Status.Armor == 0 && isArmor && !die)
            {
                //8888888888888888888888888888888888888888888888888888888888888888888888
                //破甲并且之前判断一击不死,应该触发对应事件<暂未定义,待补充>
                // await Game.OnCardArmorBreak(Card, source);
                await Game.SendEvent(new AfterCardArmorBreak(Card, source));
                //8888888888888888888888888888888888888888888888888888888888888888888888
            }
            if (isHurt && Card.Status.CardRow.IsOnPlace())
            {
                //8888888888888888888888888888888888888888888888888888888888888888888888
                //受伤并且没有进入墓地的话,应该触发对应事件<暂未定义,待补充>
                // await Game.OnCardHurt(Card, num, source);
                await Game.SendEvent(new AfterCardHurt(Card, num, source));
                //8888888888888888888888888888888888888888888888888888888888888888888888
            }
        }
        public virtual async Task Reset(GameCard source = null)//重置(未测试)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //重置,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardReset(Card, source);
            await Game.SendEvent(new AfterCardReset(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            if (Card.Status.Strength <= 0)
            {
                await Banish();
            }
        }
        public virtual async Task Heal(GameCard source = null)//治愈(未测试)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            if (Card.Status.HealthStatus < 0)
                Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //治愈,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardHeal(Card, source);
            await Game.SendEvent(new AfterCardHeal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Reveal(GameCard source = null)//揭示
        {
            //不在手上的话,或者已经被揭示,没有效果
            if (!Card.Status.CardRow.IsInHand() || Card.Status.IsReveal) return;
            Card.Status.IsReveal = true;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //揭示,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardReveal(Card, source);
            await Game.Debug($"{Card.CardInfo().Name}被揭示,发送揭示事件");
            await Game.SendEvent(new AfterCardReveal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Spying(GameCard source = null)//间谍
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status.IsSpying = !Card.Status.IsSpying;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //有卡牌间谍状态改变,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardSpyingChange(Card, Card.Status.IsSpying, source);
            if (Card.Status.IsSpying)
                await Game.SendEvent(new AfterCardSpying(Card, source));
            else
                await Game.SendEvent(new AfterCardUnSpying(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Armor(int num, GameCard source = null)//增加护甲(未测试)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || num <= 0) return;
            Card.Status.Armor += num;
            await Game.ShowCardIconEffect(Card, CardIconEffectType.MendArmor);
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //增加护甲,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardAddArmor(Card, num, source);
            await Game.SendEvent(new AfterCardAddArmor(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Conceal(GameCard source = null)//隐匿(未测试)
        {
            if (!Card.Status.CardRow.IsInHand() || !Card.Status.IsReveal) return;
            Card.Status.IsReveal = false;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //隐匿,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardConceal(Card, source);
            await Game.SendEvent(new AfterCardConceal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Resilience(GameCard source = null)//坚韧(未测试)
        {
            if (!Card.Status.CardRow.IsOnPlace()) return;
            Card.Status.IsResilience = !Card.Status.IsResilience;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //坚韧,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardResilienceChange(Card, Card.Status.IsResilience, source);
            if (Card.Status.IsResilience)
                await Game.SendEvent(new AfterCardResilience(Card, source));
            else
                await Game.SendEvent(new AfterCardUnResilience(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Discard(GameCard source = null)//丢弃(未测试)
        {//如果在场上,墓地或者已被放逐,不触发
            if (Card.Status.CardRow.IsOnPlace() || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            await ToCemetery();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //丢弃,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardDiscard(Card, source);
            await Game.SendEvent(new AfterCardDiscard(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Lock(GameCard source = null)//锁定(未测试)
        {
            if (Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status.IsLock = !Card.Status.IsLock;
            await Game.ShowSetCard(Card);
            if (Card.Status.IsLock)
            {
                if (Card.Status.IsResilience)
                    await Card.Effect.Resilience();
                if (Card.Status.Conceal)
                    await Card.Effect.Ambush();
                await Game.ShowSetCard(Card);
            }
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //锁定,应该触发对应事件<暂未定义,待补充>
            if (Card.Status.IsLock)
                await Game.SendEvent(new AfterCardLock(Card, source));
            else
                await Game.SendEvent(new AfterCardUnLock(Card, source));
            // await Game.OnCardLockChange(Card, Card.Status.IsLock, source);
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Transform(string CardId, GameCard source = null)//变为(未测试)
        {
            if (Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status = new CardStatus(CardId) { DeckFaction = Game.PlayersFaction[PlayerIndex], CardRow = Card.Status.CardRow };
            Card.Effect = (CardEffect)Activator.CreateInstance(GwentMap.CardMap[CardId].EffectType, Game, Card);
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //变为,应该触发对应事件<暂未定义,待补充>
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Resurrect(CardLocation location, GameCard source = null)//复活(未测试)
        {
            if (Card.Status.CardRow == RowPosition.Banish && !Card.Status.CardRow.IsInCemetery()) return;
            var cardCemetery = Card.PlayerIndex;
            await Game.ShowCardMove(location, Card, true);
            if (location.RowPosition.IsOnPlace())
            {
                await Game.ShowCardOn(Card);
                await Game.ClientDelay(200);
                await CardDown(false);
            }
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //复活,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardResurrect(Card, source);
            await Game.SendEvent(new AfterCardResurrect(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Charm(GameCard source = null)//被魅惑
        {
            if (!Card.Status.CardRow.IsOnPlace()) return;
            if (Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count >= 9) return;
            await Move(new CardLocation() { RowPosition = Card.Status.CardRow.Mirror(), CardIndex = Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count });
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //魅惑,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardCharm(Card, source);
            await Game.SendEvent(new AfterCardCharm(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Drain(int num, GameCard taget)//汲食
        {
            if (Card.Status.CardRow == RowPosition.Banish && Card.Status.CardRow.IsInCemetery()) return;
            var tagetNum = taget.Status.Strength + taget.Status.HealthStatus;
            num = num > tagetNum ? tagetNum : num;
            await taget.Effect.Damage(num, Card, BulletType.RedLight, true);//造成穿透伤害
            await Boost(num, Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //汲食,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardDrain(Card, num, taget);
            await Game.SendEvent(new AfterCardDrain(Card, num, taget));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Ambush()//伏击
        {
            if (!Card.Status.CardRow.IsOnPlace() && !Card.Status.Conceal) return;
            Card.Status.Conceal = false;
            await Game.ShowSetCard(Card);
            await Game.ShowCardOn(Card);
            await Game.ClientDelay(200);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //伏击,应该触发对应事件<暂未定义,待补充>
            if (!Card.Status.IsLock)
                // await Game.OnCardAmbush(Card);
                await Game.SendEvent(new AfterCardAmbush(Card));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown(false);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDownEffect(false);
        }
        public virtual async Task Consume(GameCard taget)//吞噬
        {
            if (!Card.Status.CardRow.IsOnPlace() || taget.Status.CardRow == RowPosition.Banish) return;
            var num = taget.Status.Strength + taget.Status.HealthStatus;
            //被吞噬的目标
            if (taget.Status.CardRow.IsInCemetery())
            {//如果在墓地,放逐掉
                await taget.Effect.Banish();
            }
            else if (taget.Status.CardRow.IsOnRow())
            {//如果在场上,展示吞噬动画,之后不展示动画的情况进入墓地
                await taget.Effect.ToCemetery(CardBreakEffectType.Consume);
            }
            else
            {
                await taget.Effect.ToCemetery();
            }
            await Boost(num);
            await Game.ClientDelay(500);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //吞噬,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardConsume(Card, taget);
            await Game.SendEvent(new AfterCardConsume(taget, Card));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Move(CardLocation location, GameCard source = null)
        {//移动,只限于从场上移动到另一排
            //如果不在场上,返回
            if (!Card.Status.CardRow.IsOnPlace() || Game.RowToList(Card.PlayerIndex, location.RowPosition).Count >= 9) return;
            var isSpyingChange = !location.RowPosition.IsMyRow();
            await Game.ShowCardOn(Card);
            await Game.ShowCardMove(location, Card);
            await Game.ClientDelay(200);
            await CardDown(isSpyingChange);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //位移,应该触发对应事件<暂未定义,待补充>
            // await Game.OnCardMove(Card, source);
            await Game.SendEvent(new AfterCardMove(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Summon(CardLocation location, GameCard source)//召唤到什么地方
        {   //召唤
            var isSpyingChange = !location.RowPosition.IsMyRow();
            await Game.ShowCardMove(location, Card);
            await Game.ShowCardOn(Card);
            await Game.ClientDelay(200);
            await CardDown(isSpyingChange);
        }
        //================================================================================
        //================================================================================
        //最主要要被重写的事件
        public virtual async Task<int> CardUseEffect()//特殊卡使用效果
        {
            //(await Game.GetSelectPlaceCards(1, Card)).ForAll(async x => await x.Effect.Damage(9, Card));
            await Task.CompletedTask;
            return 0;
        }
        public virtual async Task CardDownEffect(bool isSpying)//单位卡二段部署
        {
            await Task.CompletedTask;
        }
        public virtual async Task<int> CardPlayEffect(bool isSpying)
        {
            await Task.CompletedTask;
            return 0;
        }
        //-----------------------------------------------------------------
        // public virtual async Task OnTurnStart(int playerIndex) => await Task.CompletedTask;//谁的回合开始了
        // public virtual async Task OnTurnOver(int playerIndex) => await Task.CompletedTask;//谁的回合结束了
        // public virtual async Task OnRoundOver(int RoundCount, int player1Point, int player2Point) => await Task.CompletedTask;//第几轮,谁赢了
        // public virtual async Task OnPlayerPass(int playerIndex) => await Task.CompletedTask;//哪个玩家pass了
        // public virtual async Task OnCardReveal(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//揭示
        // public virtual async Task OnCardConsume(GameCard master, GameCard taget) => await Task.CompletedTask;//吞噬
        // public virtual async Task OnCardBoost(GameCard taget, int num, GameCard soure = null) => await Task.CompletedTask;//增益
        // public virtual async Task OnCardHurt(GameCard taget, int num, GameCard soure = null) => await Task.CompletedTask;//受伤
        // public virtual async Task OnSpecialPlay(GameCard taget) => await Task.CompletedTask;//法术卡使用前
        // public virtual async Task OnUnitPlay(GameCard taget) => await Task.CompletedTask;//单位卡执行一段部署前
        // public virtual async Task OnUnitDown(GameCard taget) => await Task.CompletedTask;//单位卡落下时(二段部署前)
        // public virtual async Task OnCardDeath(GameCard taget, CardLocation soure) => await Task.CompletedTask;//有卡牌进入墓地
        // public virtual async Task OnCardToCemetery(GameCard taget, CardLocation soure) => await Task.CompletedTask;//有卡牌进入墓地
        // public virtual async Task OnCardSpyingChange(GameCard taget, bool isSpying, GameCard soure = null) => await Task.CompletedTask;//场上间谍改变
        // public virtual async Task OnCardDiscard(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//卡牌被丢弃
        // public virtual async Task OnCardAmbush(GameCard taget) => await Task.CompletedTask;//有伏击卡触发
        // public virtual async Task OnCardSwap(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//卡牌交换
        // public virtual async Task OnPlayerDraw(int playerIndex, GameCard taget) => await Task.CompletedTask;//抽卡
        // public virtual async Task OnCardConceal(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//隐匿
        // public virtual async Task OnCardLockChange(GameCard taget, bool isLock, GameCard soure = null) => await Task.CompletedTask;//锁定状态改变
        // public virtual async Task OnCardAddArmor(GameCard taget, int num, GameCard soure = null) => await Task.CompletedTask;//增加护甲
        // public virtual async Task OnCardSubArmor(GameCard taget, int num, GameCard soure = null) => await Task.CompletedTask;//降低护甲
        // public virtual async Task OnCardArmorBreak(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//护甲被破坏
        // public virtual async Task OnCardResurrect(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//有卡牌复活
        // public virtual async Task OnCardResilienceChange(GameCard taget, bool isResilience, GameCard soure = null) => await Task.CompletedTask;//坚韧状态改变
        // public virtual async Task OnWeatherApply(int playerIndex, int row, RowStatus type) => await Task.CompletedTask;//有天气降下
        // public virtual async Task OnCardHeal(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//卡牌被治愈
        // public virtual async Task OnCardReset(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//卡牌被重置
        // public virtual async Task OnCardStrengthen(GameCard taget, int num, GameCard soure = null) => await Task.CompletedTask;//强化
        // public virtual async Task OnCardWeaken(GameCard taget, int num, GameCard soure = null) => await Task.CompletedTask;//削弱
        // public virtual async Task OnCardDrain(GameCard master, int num, GameCard taget) => await Task.CompletedTask;//有单位汲食时
        // public virtual async Task OnCardCharm(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//被魅惑
        // public virtual async Task OnCardMove(GameCard taget, GameCard soure = null) => await Task.CompletedTask;//移动
    }
}