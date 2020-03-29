using System.Diagnostics;
using System;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Extensions;

namespace Cynthia.Card
{
    public abstract class CardEffect : Effect, IHandlesEvent<CardUseEffect>, IHandlesEvent<CardPlayEffect>, IHandlesEvent<CardDownEffect>
    {
        public CardEffect(GameCard card)
        {
            Game = card.Game;
            Card = card;
        }

        //必要的两个信息对象
        public GameCard Card { get; set; }//宿主
        public IGwentServerGame Game { get; set; }//游戏本体

        //为了方便而添加的属性和方法
        public int AnotherPlayer { get => Game.AnotherPlayer(Card.PlayerIndex); }
        public int PlayerIndex { get => Card.PlayerIndex; }
        public RowPosition MyRow { get => Card.Status.CardRow; }
        public int Countdown { get => Card.Status.Countdown; }
        public Random RNG { get => Game.RNG; }
        public async Task SetCountdown(int? value = default, int? offset = default)
        {
            await Game.ShowCardNumberChange(Card, Card.Status.Countdown, NumberType.Countdown);
            Card.Status.Countdown = (value ?? Card.Status.Countdown) + (offset ?? 0);
            if (Card.Status.Countdown <= 0)
                Card.Status.IsCountdown = false;
            else
                Card.Status.IsCountdown = true;
            await Game.ShowSetCard(Card);
        }
        //----------------------------------------------------------
        //未完成迁移的事件
        public async Task HandleEvent(CardDownEffect @event)
        {
            await CardDownEffect(@event.IsSpying, @event.IsReveal);
        }
        public async Task HandleEvent(CardPlayEffect @event)
        {
            @event.SearchCount += await CardPlayEffect(@event.IsSpying, @event.IsReveal);

        }
        public async Task HandleEvent(CardUseEffect @event)
        {
            @event.SearchCount += await CardUseEffect();
        }

        public virtual async Task<int> CardUseEffect()//特殊卡使用效果
        {
            await Task.CompletedTask;
            return 0;
        }
        public virtual async Task CardDownEffect(bool isSpying, bool isReveal)//单位卡二段部署
        {
            await Task.CompletedTask;
        }
        public virtual async Task<int> CardPlayEffect(bool isSpying, bool isReveal)
        {
            await Task.CompletedTask;
            return 0;
        }
        //-----------------------------------------------------------
        //指令1

        //使卡牌"使用"
        public virtual async Task CardUse()
        {
            var count = 0;
            var result = await CardUseStart();
            //历史卡牌
            Game.HistoryList.Add((Card.PlayerIndex, Card));
            if (result)
            {
                if (Card.Status.CardRow.IsOnStay())
                    // count = await CardUseEffect();
                    await Game.AddTask(async () =>
                    {
                        count = ((CardUseEffect)await Card.Effects.RaiseEvent(new CardUseEffect())).SearchCount;
                    });
            }
            if (Card.Status.CardRow.IsOnStay())
                await CardUseEnd();
            count = (await Game.SendEvent(new BeforePlayStayCard(Card, count))).PlayCount;
            await PlayStayCard(count, false);
        }

        //使卡牌"放置"
        public virtual async Task Play(CardLocation location, bool forceSpying = false, bool isFromHand = true)//放置
        {
            await Game.AddTask(async () =>
            {
                await Game.SendEvent(new BeforeUnitPlay(Card, isFromHand));
            });
            var (isSpying, isReveal) = await CardPlayStart(location, isFromHand);
            isSpying |= forceSpying;
            //历史卡牌
            // Game.HistoryList.Add((isSpying ? AnotherPlayer : Card.PlayerIndex, Card));
            if (Card.Status.Conceal)
            {
                //如果是伏击
                await Game.ShowCardDown(Card);
                return;
            }
            await Game.SendEvent(new AfterUnitPlay(Card, isFromHand, isSpying));
            var count = 0;
            if (Card.Status.CardRow.IsOnPlace())
                // count = await CardPlayEffect(isSpying);
                await Game.AddTask(async () =>
                {
                    count = ((CardPlayEffect)await Card.Effects.RaiseEvent(new CardPlayEffect(isSpying, isReveal))).SearchCount;
                });
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown(isSpying, isFromHand, false, (false, false));
            count = (await Game.SendEvent(new BeforePlayStayCard(Card, count))).PlayCount;
            await PlayStayCard(count, isSpying);
            if (Card.Status.CardRow.IsOnPlace())
                await Game.AddTask(async () =>
                {
                    await Card.Effects.RaiseEvent(new CardDownEffect(isSpying, isReveal));
                });
        }

        //使卡牌"进入墓地"
        public virtual Task ToCemetery(CardBreakEffectType type = CardBreakEffectType.ToCemetery)
        {
            return ToCemetery(discardInfo: (false, null), isRoundEnd: false, type: type);
        }
        private async Task ToCemetery((bool isDiscard, GameCard discardSource) discardInfo, bool isRoundEnd, CardBreakEffectType type = CardBreakEffectType.ToCemetery)
        {
            var isDead = Card.Status.CardRow.IsOnPlace();
            var deadposition = Game.GetCardLocation(Card);
            
            //进入墓地后撤销护盾
            Card.Status.IsShield=false;

            //立刻执行,将卡牌视作僵尸卡
            if (Card.CardPoint() != 0 && Card.Status.CardRow.IsOnPlace())
            {
                Card.Status.HealthStatus = -Card.Status.Strength;
            }
            await Game.ShowSetCard(Card);

            //延迟执行1.卡牌抬起
            await Game.AddTask(async () =>
            {
                //如果是特殊死亡,播放动画
                if (type != CardBreakEffectType.ToCemetery)
                {
                    await Game.AddTask(async () =>
                    {
                        await Game.ShowCardBreakEffect(Card, type);
                        var row = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow);
                        var target = Game.RowToList(Card.PlayerIndex, RowPosition.MyCemetery);
                        await Game.LogicCardMove(Card, target, 0);
                        Repair();
                        await Game.AddTask((Func<Task>)sendEventTask);
                    });
                }
                //如果是移动进入墓地
                else
                {
                    //1.抬起
                    if (Card.Status.CardRow.IsOnPlace())
                    {
                        await Game.ShowCardOn(Card);
                        await Game.ClientDelay(50);
                        if (Card.Status.Strength <= 0 && Card.Status.Type == CardType.Unit)
                        {
                            await Banish();
                            return;
                        }
                    }
                    await Game.AddTask(async () =>
                    {
                        //2.移动到墓地并且恢复
                        await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyCemetery, CardIndex = 0 }, Card, autoUpdateCemetery: !isRoundEnd);//如果是收场,进行统一处理,这里屏蔽掉更新墓地
                        Repair();
                        //3.遗愿和事件
                        await Game.AddTask((Func<Task>)sendEventTask);
                    });
                }
            });
            async Task sendEventTask()
            {
                if (Card.Status.IsDoomed || (Card.Status.Strength <= 0 && Card.Status.Type == CardType.Unit))//如果是佚亡,放逐
                {
                    await Banish();
                    return;
                }
                if (Card.Status.CardRow != RowPosition.Banish)
                    await Game.SendEvent(new AfterCardToCemetery(Card, deadposition, isRoundEnd));
                //8888888888888888888888888888888888888888888888888888888888888888888888
                //进入墓地(遗愿),应该触发对应事件<暂未定义,待补充>
                if (!isRoundEnd)
                {
                    if (isDead && Card.Status.CardRow != RowPosition.Banish)//如果从场上进入墓地,并且没有被放逐
                    {
                        await Game.SendEvent(new AfterCardDeath(Card, deadposition));
                    }
                    else if (discardInfo.isDiscard && !deadposition.RowPosition.IsOnPlace())
                    {
                        await Game.SendEvent(new AfterCardDiscard(Card, discardInfo.discardSource));
                    }
                }
                //8888888888888888888888888888888888888888888888888888888888888888888888
                if (Card.HasAnyCategorie(Categorie.Token))
                {
                    await Game.AddTask(Banish);
                }
                await Game.SetPointInfo();
                await Game.SetCountInfo();
            }
        }

        //使卡牌"恢复状态"
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

            Card.Status.IsImmue = false;//没有免疫
        }

        //使卡牌"放逐"
        public virtual async Task Banish()
        {
            var banishPosition = Game.GetCardLocation(Card);
            await Game.SendEvent(new BeforeCardBanish(Card, banishPosition));
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

        //使卡牌"小局结束"
        public virtual async Task RoundEnd()
        {   //当回合结束的时候,如果在场上,进行处理
            if (!Card.Status.CardRow.IsOnPlace()) return;

            if (Card.Status.IsResilience)
            {
                Card.Status.Armor = 0; //护甲归零
                if (Card.Status.HealthStatus >= 0)//没有受伤
                    Card.Status.HealthStatus = 0;
                await Card.Effect.Resilience(Card);
                return;
            }
            await ToCemetery(discardInfo: (false, null), isRoundEnd: true);
        }
        //---------------------------------------------------------------------------
        //特殊卡使用的分布方法
        public virtual async Task<bool> CardUseStart()//使用前移动
        {
            Card.Status.IsReveal = false;//不管怎么样,都先设置成非揭示状态
            if (!Card.Status.CardRow.IsOnStay())
                await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            await Game.ClientDelay(200);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //打出了特殊牌,应该触发对应事件
            return (await Game.SendEvent(new BeforeSpecialPlay(Card))).IsUse;
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task CardUseEnd()//使用结束
        {
            await Game.ClientDelay(300);
            await ToCemetery();
        }
        //----------------------------------------------------------------------------
        //单位卡放置的分步方法
        public virtual async Task<(bool isSpying, bool isReveal)> CardPlayStart(CardLocation location, bool isFromHand)//先是移动到目标地点
        {
            var isSpying = !location.RowPosition.IsMyRow();
            var IsReveal = Card.Status.IsReveal;
            Card.Status.IsReveal = false;//不管怎么样,都先设置成非揭示状态
            if (location.RowPosition != Card.Status.CardRow || Card.GetRowIndex() != location.CardIndex)
            {
                await Game.ShowCardOn(Card);
                await Game.ShowCardMove(location, Card);
                await Game.ClientDelay(400);
            }
            return (isSpying, IsReveal);//有没有间谍呢
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
                        await Game.PlayersStay[stayPlayer][0].Effect.Play(location, false, false);
                }
            }
        }
        public virtual async Task CardDown(bool isSpying, bool isFromHand, bool isFromPlance, (bool isMove, bool isFromEnemy) isMoveInfo)
        {
            await Game.ShowCardDown(Card);
            await Game.SetPointInfo();
            if (isSpying)
                await Spying(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //打出了卡牌,应该触发对应事件<暂未定义,待补充>
            await Game.AddTask(async () => await Game.SendEvent(new AfterUnitDown(Card, isFromHand, isFromPlance, isMoveInfo, isSpying)));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            if (!isMoveInfo.isMove)
                Game.HistoryList.Add((isSpying ? AnotherPlayer : Card.PlayerIndex, Card));
            //-----------------------------------------
            //大概,判断天气陷阱一类的(血月坑陷)(已经交给游戏事件处理)
        }
        //=====================================================================================================
        //指令2
        public virtual async Task Strengthen(int num, GameCard source)//强化
        {
            if (num <= 0 || Card.IsDead || Card.Status.Type == CardType.Special) return;
            if (source != null)
            {
                await Game.ShowBullet(source, Card, BulletType.GreenLight);
            }
            await Game.ShowCardNumberChange(Card, num, NumberType.White);
            await Game.ClientDelay(50);
            Card.Status.Strength += num;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //await Game.ClientDelay(150);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //强化,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardStrengthen(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Weaken(int num, GameCard source)//削弱
        {
            if (num <= 0 || Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            //此为最大承受值
            var bear = Card.Status.Strength;
            if (num > bear) num = bear;
            if (source != null)
            {
                await Game.ShowBullet(source, Card, BulletType.RedLight);
            }
            //最大显示的数字,不超过这个值
            var showBear = Card.Status.Strength + Card.Status.HealthStatus;
            await Game.ShowCardNumberChange(Card, num > showBear ? -showBear : -num, NumberType.White);
            await Game.ClientDelay(50);
            Card.Status.Strength -= num;
            if (Card.Status.Strength < -Card.Status.HealthStatus) Card.Status.HealthStatus = -Card.Status.Strength;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //await Game.ClientDelay(150);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //有单位被削弱了,应该触发对应事件<暂未定义,待补充>
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
        public virtual async Task Boost(int num, GameCard source)//增益
        {
            if (num <= 0 || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            if (source != null)
            {
                await Game.ShowBullet(source, Card, BulletType.GreenLight);
            }
            await Game.ShowCardNumberChange(Card, num, NumberType.Normal);
            await Game.ClientDelay(50);
            Card.Status.HealthStatus += num;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //await Game.ClientDelay(150);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //有卡牌增益,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardBoost(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Damage(int num, GameCard source, BulletType showType = BulletType.Arrow, bool isPenetrate = false, DamageType damageType = DamageType.Unit)//伤害
        {
            if (num <= 0 || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.Status.Type != CardType.Unit || Card.IsDead || Card.Status.Type == CardType.Special) return;

            var beforeEventPackage = await Game.SendEvent(new BeforeCardDamage(Card, num, source, damageType));

            //可能因为某些卡牌效果取消
            if (beforeEventPackage.IsCancel)
            {
                return;
            }

            //可能因为某些卡牌效果重定向目标,或改变伤害数值,伤害类型
            num = beforeEventPackage.Num;
            source = beforeEventPackage.Source;
            damageType = beforeEventPackage.DamageType;

            //如果有护盾，取消这一次的伤害
            if(Card.Status.IsShield)
            {
                await Game.ShowSetCard(Card);
                Card.Status.IsShield=false;
                return;
            }

            if (num <= 0 || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.Status.Type != CardType.Unit || Card.IsDead || Card.Status.Type == CardType.Special) return;

            //最高承受伤害,如果穿透的话,不考虑护甲
            var die = false;
            var isArmor = Card.Status.Armor > 0;
            var bear = (Card.Status.Strength + (isPenetrate ? 0 : Card.Status.Armor) + Card.Status.HealthStatus);
            if (num >= bear)
            {
                num = bear;//如果数值大于最高伤害的话,进行限制
                die = true;//死亡,不会触发任何效果
            }
            if (Card.Status.CardRow.IsInHand() && num >= Card.CardPoint())
            {
                num = Card.CardPoint() - 1;
                if (num <= 0) return;
            }
            if (source != null)
            {
                await Game.ShowBullet(source, Card, showType);
            }
            //--------------------------------------------------------------
            //护甲处理
            if (Card.Status.Armor > 0 && !isPenetrate)//如果有护甲并且并非穿透伤害
            {
                //首先播放破甲动画
                await Game.ShowCardIconEffect(Card, CardIconEffectType.BreakArmor);
                if (Card.Status.Armor >= num)//如果护甲更高的话
                {
                    //8888888888888888888888888888888888888888888888888888888888888888888888
                    //护甲值降低,应该触发对应事件<暂未定义,待补充>
                    await Game.SendEvent(new AfterCardSubArmor(Card, num, source));
                    //8888888888888888888888888888888888888888888888888888888888888888888888
                    Card.Status.Armor -= num;
                    await Game.ShowSetCard(Card);//更新客户端的护甲值
                    num = 0;
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
            if (Card.Status.Armor == 0 && isArmor && !die)
            {
                //8888888888888888888888888888888888888888888888888888888888888888888888
                //破甲并且之前判断一击不死,应该触发对应事件<暂未定义,待补充>
                await Game.SendEvent(new AfterCardArmorBreak(Card, source));
                //8888888888888888888888888888888888888888888888888888888888888888888888
            }
            var isHurt = num > 0;
            if (num > 0)
            {
                await Game.ShowCardNumberChange(Card, -num, NumberType.Normal);
                await Game.ClientDelay(50);
                Card.Status.HealthStatus -= num;
                await Game.ShowSetCard(Card);
                await Game.SetPointInfo();
            }
            if (isHurt && Card.Status.CardRow.IsOnPlace())
            {
                //8888888888888888888888888888888888888888888888888888888888888888888888
                //受伤并且没有进入墓地的话,应该触发对应事件<暂未定义,待补充>
                await Game.SendEvent(new AfterCardHurt(Card, num, source, damageType));
                //8888888888888888888888888888888888888888888888888888888888888888888888
            }
            if ((Card.CardPoint()) <= 0)
            {
                await ToCemetery();
                return;
            }
        }
        public virtual async Task Reset(GameCard source)//重置
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //重置,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardReset(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            if (Card.Status.Strength <= 0 && Card.Status.Type == CardType.Unit)
            {
                await Banish();
            }
        }
        public virtual async Task Heal(GameCard source)//治愈
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            if (Card.Status.HealthStatus < 0)
                Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //治愈,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardHeal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Reveal(GameCard source)//揭示
        {
            //不在手上的话,或者已经被揭示,没有效果
            if (!Card.Status.CardRow.IsInHand() || Card.Status.IsReveal) return;
            Card.Status.IsReveal = true;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //揭示,应该触发对应事件<暂未定义,待补充>
            await Game.Debug($"{Card.CardInfo().Name}被揭示,发送揭示事件");
            await Game.SendEvent(new AfterCardReveal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Spying(GameCard source)//间谍
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.IsSpying = !Card.Status.IsSpying;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //有卡牌间谍状态改变,应该触发对应事件<暂未定义,待补充>
            if (Card.Status.IsSpying)
                await Game.SendEvent(new AfterCardSpying(Card, source));
            else
                await Game.SendEvent(new AfterCardUnSpying(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Armor(int num, GameCard source)//增加护甲
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || num <= 0 || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.Armor += num;
            await Game.ShowCardIconEffect(Card, CardIconEffectType.MendArmor);
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //增加护甲,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardAddArmor(Card, num, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Conceal(GameCard source)//隐匿(未测试)
        {
            if (!Card.Status.CardRow.IsInHand() || !Card.Status.IsReveal || Card.IsDead) return;
            Card.Status.IsReveal = false;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //隐匿,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardConceal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }

        public virtual async Task PlanceConceal(GameCard source)//伏击(未测试)
        {
            if (!(Card.Status.CardRow.IsOnPlace() || Card.Status.CardRow.IsInHand() || Card.Status.CardRow.IsOnStay()) || Card.Status.Conceal || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.Conceal = true;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //隐匿,应该触发对应事件<暂未定义,待补充>
            // await Game.SendEvent(new AfterCardPlanceConceal(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }

        public virtual async Task Resilience(GameCard source)//坚韧
        {
            if (!Card.Status.CardRow.IsOnPlace() || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.IsResilience = !Card.Status.IsResilience;
            await Game.ShowSetCard(Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //坚韧,应该触发对应事件<暂未定义,待补充>
            if (Card.Status.IsResilience)
                await Game.SendEvent(new AfterCardResilience(Card, source));
            else
                await Game.SendEvent(new AfterCardUnResilience(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Discard(GameCard source)//丢弃(未测试)
        {//如果在场上,墓地或者已被放逐,不触发
            if (Card.Status.CardRow.IsOnPlace() || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.IsDead) return;
            await ToCemetery(discardInfo: (true, source), isRoundEnd: false);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //丢弃,应该触发对应事件(在ToCemetery内部触发)
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Lock(GameCard source)//锁定
        {
            if (Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.IsLock = !Card.Status.IsLock;
            await Game.ShowSetCard(Card);
            if (Card.Status.IsLock)
            {
                if (Card.Status.IsResilience)
                    await Card.Effect.Resilience(source);
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
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Transform(string cardId, GameCard source, Action<GameCard> setting = null, bool isForce = false)//变为
        {
            setting ??= (x => { });
            if (Card.Status.CardId == cardId && !isForce)
            {
                return;
            }
            if (Card.Status.CardRow == RowPosition.Banish || Card.IsDead) return;
            Card.Status = new CardStatus(cardId) { DeckFaction = Game.PlayersFaction[PlayerIndex], CardRow = Card.Status.CardRow };
            Card.Effects.Clear();
            Card.Effects.Add(Game.CreateEffectInstance(cardId, Card));
            setting(Card);
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //变为,应该触发对应事件<暂未定义,待补充>
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Resurrect(CardLocation location, GameCard source)//复活
        {
            if (Card.Status.CardRow == RowPosition.Banish || !Card.Status.CardRow.IsInCemetery()) return;
            if (Card.Status.IsLock)
            {
                await Card.Effect.Lock(source);
            }
            var rowCards = Game.RowToList(Card.PlayerIndex, location.RowPosition);
            if (location.RowPosition.IsOnPlace() && rowCards.Count >= Game.RowMaxCount) return;
            if (location.CardIndex > rowCards.Count)
            {
                location.CardIndex = rowCards.Count;
            }
            await Game.ShowCardMove(location, Card, true);
            if (location.RowPosition.IsOnPlace())
            {
                await Game.ShowCardOn(Card);
                await Game.AddTask(async () =>
                {
                    await CardDown(false, false, false, (false, false));
                });
            }
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //复活,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardResurrect(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Charm(GameCard source)//被魅惑
        {
            if (!Card.Status.CardRow.IsOnPlace() || Card.IsDead || Card.Status.Type == CardType.Special) return;
            if (Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count >= Game.RowMaxCount) return;
            await Move(new CardLocation() { RowPosition = Card.Status.CardRow.Mirror(), CardIndex = Game.RowToList(AnotherPlayer, Card.Status.CardRow).Count }, Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //魅惑,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardCharm(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Drain(int num, GameCard target)//汲食
        {
            if (Card.Status.CardRow == RowPosition.Banish || Card.Status.CardRow.IsInCemetery() || Card.IsDead) return;
            var tagetNum = target.Status.Strength + target.Status.HealthStatus;
            num = num > tagetNum ? tagetNum : num;
            await target.Effect.Damage(num, Card, BulletType.RedLight, true);//造成穿透伤害
            await Boost(num, Card);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //汲食,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardDrain(Card, num, target));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Ambush(Func<Task> ambushEffect = null)//伏击
        {
            ambushEffect ??= async () => { await Task.CompletedTask; };
            if (!(Card.Status.CardRow.IsOnPlace() && Card.Status.Conceal) || Card.IsDead || Card.Status.Type == CardType.Special) return;
            Card.Status.Conceal = false;
            await Game.ShowSetCard(Card);
            await Game.ShowCardOn(Card);
            await Game.ClientDelay(200);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //伏击,应该触发对应事件<暂未定义,待补充>
            if (!Card.Status.IsLock)
            {
                await ambushEffect();
                await Game.SendEvent(new AfterCardAmbush(Card));
            }
            //8888888888888888888888888888888888888888888888888888888888888888888888
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown(false, false, false, (false, false));
            if (Card.Status.CardRow.IsOnPlace())
                // await CardDownEffect(false);
                await Game.AddTask(async () => await Card.Effects.RaiseEvent(new CardDownEffect(false, false)));

        }
        public virtual async Task Consume(GameCard target, Func<GameCard, int> consumePoint = null)//吞噬
        {
            consumePoint ??= (x => x.CardPoint());
            if (!Card.Status.CardRow.IsOnPlace() || target.Status.CardRow == RowPosition.Banish || Card.IsDead) return;
            // var num = target.Status.Strength + target.Status.HealthStatus;
            //被吞噬的目标
            var point = consumePoint(target);
            if (target.Status.CardRow.IsInCemetery())
            {//如果在墓地,放逐掉
                await target.Effect.Banish();
            }
            else if (target.Status.CardRow.IsOnRow())
            {//如果在场上,展示吞噬动画,之后不展示动画的情况进入墓地
                await target.Effect.ToCemetery(CardBreakEffectType.Consume);
            }
            else
            {
                await target.Effect.ToCemetery();
            }
            await Boost(point, target);
            await Game.ClientDelay(500);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //吞噬,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardConsume(target, Card));
            //8888888888888888888888888888888888888888888888888888888888888888888888
        }
        public virtual async Task Move(CardLocation location, GameCard source)//发送位移
        {//移动,只限于从场上移动到另一排
            //如果不在场上,返回
            var count = Game.RowToList(Card.PlayerIndex, location.RowPosition).Count;
            if (!Card.Status.CardRow.IsOnPlace() || count >= Game.RowMaxCount || Card.IsDead || Card.Status.Type == CardType.Special) return;
            if (location.CardIndex > count)
            {
                location.CardIndex = count;
            }
            var isSpyingChange = !location.RowPosition.IsMyRow();
            await Game.ShowCardOn(Card);
            await Game.ShowCardMove(location, Card);
            await Game.ClientDelay(200);
            //8888888888888888888888888888888888888888888888888888888888888888888888
            //位移,应该触发对应事件<暂未定义,待补充>
            await Game.SendEvent(new AfterCardMove(Card, source));
            //8888888888888888888888888888888888888888888888888888888888888888888888
            await Game.AddTask(async () => await CardDown(isSpyingChange, false, false, (true, isSpyingChange)));
        }
        public virtual async Task Summon(CardLocation location, GameCard source)//召唤到什么地方
        {   //召唤
            var rowCards = Game.RowToList(Card.PlayerIndex, location.RowPosition);
            if (rowCards.Count >= Game.RowMaxCount || Card.IsDead) return;
            if (location.CardIndex > rowCards.Count)
            {
                location.CardIndex = rowCards.Count;
            }
            var isSpyingChange = !location.RowPosition.IsMyRow();
            if (Card.Status.IsReveal) Card.Status.IsReveal = false;
            await Game.ShowCardMove(location, Card);
            await Game.ShowCardOn(Card);
            await Game.AddTask(async () =>
            {
                await Game.ClientDelay(200);
                await CardDown(isSpyingChange, false, false, (false, false));
            });
        }

        public virtual async Task Duel(GameCard target, GameCard source)
        {
            if (target.IsDead || !target.Status.CardRow.IsOnPlace() || Card.IsDead || !Card.Status.CardRow.IsOnPlace() || target.Status.Type != CardType.Unit || Card.Status.Type != CardType.Unit || Card.IsDead)
                return;
            int count = 0;
            while (true)
            {
                count++;
                await target.Effect.Damage(Card.CardPoint(), Card, BulletType.RedLight);
                if (target.IsDead || !target.Status.CardRow.IsOnPlace()) return;
                await Game.ClientDelay(400);
                await Card.Effect.Damage(target.CardPoint(), target, BulletType.RedLight);
                if (Card.IsDead || !Card.Status.CardRow.IsOnPlace()) return;
                await Game.ClientDelay(400);
                if (count > 20) return;
            }
        }

        public virtual async Task Swap()
        {
            if (!Card.Status.CardRow.IsInHand())
            {
                return;
            }
            Card.Status.IsReveal = false;
            await Game.ShowCardMove(new CardLocation(RowPosition.MyDeck, Game.RNG.Next(0, Game.PlayersDeck[PlayerIndex].Count() + 1)), Card);
            await Game.SendEvent(new AfterCardSwap(Card));
        }

        public virtual async Task Swap(GameCard target)
        {
            if (!Card.Status.CardRow.IsInHand() || !target.Status.CardRow.IsInDeck())
            {
                return;
            }
            await Swap();
            await GetDeckSwapCard(target);
        }

        public virtual async Task GetDeckSwapCard(GameCard target)
        {
            await Game.PlayerDrawCard(Card.PlayerIndex, filter: x => x == target);
            await Game.SendEvent(new AfterDrawSwapCard(Card));
        }

        public virtual async Task Reply(int num, GameCard source)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || Card.IsDead || Card.Status.Type == CardType.Special) return;
            num = Math.Abs(Card.Status.HealthStatus) < num ? Math.Abs(Card.Status.HealthStatus) : num;
            if (Card.Status.HealthStatus < 0)
                Card.Status.HealthStatus += num;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
        }
        //================================================================================
    }
}