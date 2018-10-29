using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Alsein.Utilities;

namespace Cynthia.Card
{
    public class CardEffect
    {
        public GameCard Card { get; set; }//宿主
        public IGwentServerGame Game { get; set; }//游戏本体
        public int AnotherPlayer { get => Game.AnotherPlayer(Card.PlayerIndex); }

        public CardEffect(IGwentServerGame game, GameCard card)
        {
            Game = game;
            Card = card;
        }

        //-----------------------------------------------------------
        //公共效果
        public virtual async Task ToCemetery(bool isShowToCemetery = true)//进入墓地触发
        {
            Card.Status.Armor = 0; //护甲归零
            Card.Status.HealthStatus = 0;//没有增益和受伤
            Card.Status.IsCardBack = false; //没有背面
            Card.Status.IsResilience = false;//没有坚韧
            Card.Status.IsShield = false; //没有昆恩
            Card.Status.IsSpying = false; //没有间谍
            Card.Status.Conceal = false;  //没有隐藏
            Card.Status.IsReveal = false; //没有揭示
            if (isShowToCemetery)
            {
                if (Card.Status.CardRow.IsOnPlace())
                {
                    await Game.ShowCardOn(Card);
                    await Task.Delay(200);
                    await Game.ShowSetCard(Card);
                    await Task.Delay(200);
                    if (Card.Status.Strength <= 0)
                    {
                        await Banish();
                        return;
                    }
                }
                await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyCemetery, CardIndex = 0 }, Card);
                await Task.Delay(400);
            }
            else
            {
                var row = Game.RowToList(Card.PlayerIndex, Card.Status.CardRow);
                var taget = Game.RowToList(Card.PlayerIndex, RowPosition.MyCemetery);
                Game.LogicCardMove(row, row.IndexOf(Card), taget, 0);
            }
            if (Card.Status.IsDoomed)//如果是佚亡,放逐
            {
                await Banish();
                return;
            }
            await Game.SetCemeteryInfo(Card.PlayerIndex);
            await Game.SetPointInfo();
            //***************************************
            //进入墓地(遗愿),应该触发对应事件<暂未定义,待补充>
            //***************************************
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
            await Game.SetPointInfo();
            await Game.SetCemeteryInfo(Card.PlayerIndex);
        }

        //-----------------------------------------------------------
        //特殊卡的单卡使用
        public virtual async Task CardUse()//使用
        {
            await CardUseStart();
            await CardUseEffect();
            await CardUseEnd();
        }
        public virtual async Task CardUseStart()//使用前移动
        {
            Card.Status.IsReveal = false;//不管怎么样,都先设置成非揭示状态
            await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyStay, CardIndex = 0 }, Card);
            await Task.Delay(200);
            //***************************************
            //打出了特殊牌,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task CardUseEffect()//使用效果
        {
            (await Game.GetSelectPlaceCards(1, Card)).ForAll(async x => await x.Effect.Damage(9, Card));
        }
        public virtual async Task CardUseEnd()//使用结束
        {
            await Task.Delay(300);
            await ToCemetery();
        }

        //----------------------------------------------------------------------------
        //单位卡的单卡放置
        public virtual async Task Play(CardLocation location)//放置
        {
            var isSpying = await CardPlayStart(location);
            var count = await CardPlayEffect(isSpying);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown(isSpying);
            await PlayStayCard(count);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDownEffect();
        }
        public virtual async Task<bool> CardPlayStart(CardLocation location)//先是移动到目标地点
        {
            var isSpying = !location.RowPosition.IsMyRow();
            Card.Status.IsReveal = false;//不管怎么样,都先设置成非揭示状态
            await Game.ShowCardOn(Card);
            await Game.ShowCardMove(location, Card);
            await Task.Delay(400);
            return isSpying;//有没有间谍呢
        }
        public virtual async Task PlayStayCard(int count)
        {
            for (var i = 0; i < count; i++)
            {
                var location = await Game.GetPlayCard(Game.PlayersStay[Card.PlayerIndex][0]);
                await Game.PlayersStay[Card.PlayerIndex][0].Effect.Play(location);
            }
        }
        public virtual async Task<int> CardPlayEffect(bool isSpying)
        {
            if (!isSpying)
            {
                /*侦查/特使: 从卡组随机选择两张铜色单位,选择一张打出
                var cardlist = Game.PlayersDeck[Card.PlayerIndex]
                .Where(x => x.Status.Group == Group.Copper && x.CardInfo().CardType == CardType.Unit)
                .Mess().Take(2).ToList();//铜色单位卡,乱序取2
                if (cardlist.Count() == 0) return 0;
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, cardlist);
                if (result.Count() == 0) return 0;
                await result.Single().MoveToCardStayFirst();
                return 1;*/
                var cardlist = Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)].Concat(Game.PlayersHandCard[Card.PlayerIndex])
                .Where(x => x.Status.IsReveal == false).ToList();
                if (cardlist.Count() == 0) return 0;
                var result = await Game.GetSelectMenuCards(Card.PlayerIndex, cardlist, 4, isEnemyBack: true);
                if (result.Count() == 0) return 0;
                result.ForAll(async x =>
                {
                    await x.Effect.Reveal(Card);
                    await x.Effect.Boost(2);
                });
            }
            else
            {
                await Boost(4);
            }
            return 0;
        }
        public virtual async Task CardDown(bool isSpying)
        {
            await Game.ShowCardDown(Card);
            await Game.SetPointInfo();
            if (isSpying)
                await Spying();
            //***************************************
            //打出了卡牌,应该触发对应事件<暂未定义,待补充>
            //***************************************
            //-----------------------------------------
            //大概,判断天气陷阱一类的(血月坑陷)
        }
        public virtual async Task CardDownEffect()//卡牌落下效果
        {
            await Task.CompletedTask;
        }
        public virtual async Task BigRoundEnd()//小局结束
        {
            await Task.CompletedTask;
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
            await Task.Delay(50);
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            await Task.Delay(150);
            //***************************************
            //强化,应该触发对应事件<暂未定义,待补充>
            //***************************************
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
            await Task.Delay(50);
            if (Card.Status.Strength < -Card.Status.HealthStatus) Card.Status.HealthStatus = -Card.Status.Strength;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            await Task.Delay(150);
            //***************************************
            //有单位被削弱了,应该触发对应事件<暂未定义,待补充>
            //***************************************
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
            await Task.Delay(50);
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            await Task.Delay(150);
            //***************************************
            //有卡牌增益,应该触发对应事件<暂未定义,待补充>
            //***************************************
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
                    num = 0;
                    //***************************************
                    //护甲值降低,应该触发对应事件<暂未定义,待补充>
                    //***************************************
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
                await Task.Delay(50);
                await Game.ShowSetCard(Card);
                await Game.SetPointInfo();
                await Task.Delay(150);
                if ((Card.Status.HealthStatus + Card.Status.Strength) <= 0)
                {
                    await ToCemetery();
                    return;
                }
            }
            if (Card.Status.Armor == 0 && isArmor && !die)
            {
                //***************************************
                //破甲并且之前判断一击不死,应该触发对应事件<暂未定义,待补充>
                //***************************************
            }
            if (isHurt && Card.Status.CardRow.IsOnPlace())
            {
                //***************************************
                //受伤并且没有进入墓地的话,应该触发对应事件<暂未定义,待补充>
                //***************************************
            }
        }
        public virtual async Task Reset(GameCard taget = null)//重置(未测试)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //***************************************
            //重置,应该触发对应事件<暂未定义,待补充>
            //***************************************
            if (Card.Status.Strength <= 0)
            {
                await Banish();
            }
        }
        public virtual async Task Heal(GameCard taget = null)//治愈(未测试)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            if (Card.Status.HealthStatus < 0)
                Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //***************************************
            //治愈,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Reveal(GameCard source = null)//揭示
        {
            //不在手上的话,或者已经被揭示,没有效果
            if (!Card.Status.CardRow.IsInHand() || Card.Status.IsReveal) return;
            Card.Status.IsReveal = true;
            await Game.ShowSetCard(Card);
            //***************************************
            //揭示,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Spying(GameCard source = null)//间谍
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status.IsSpying = !Card.Status.IsSpying;
            await Game.ShowSetCard(Card);
            //***************************************
            //有卡牌间谍状态改变,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Armor(int num, GameCard source = null)//增加护甲(未测试)
        {
            if (Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish || num <= 0) return;
            Card.Status.Armor += num;
            await Game.ShowCardIconEffect(Card, CardIconEffectType.MendArmor);
            await Game.ShowSetCard(Card);
            //***************************************
            //增加护甲,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Conceal(GameCard source = null)//隐匿(未测试)
        {
            if (!Card.Status.CardRow.IsInHand() || !Card.Status.IsReveal) return;
            Card.Status.IsReveal = false;
            await Game.ShowSetCard(Card);
            //***************************************
            //隐匿,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Resilience(GameCard source = null)//坚韧(未测试)
        {
            if (!Card.Status.CardRow.IsOnPlace()) return;
            Card.Status.IsResilience = !Card.Status.IsResilience;
            await Game.ShowSetCard(Card);
            //***************************************
            //坚韧,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Discard(GameCard source = null)//丢弃(未测试)
        {//如果在场上,墓地或者已被放逐,不触发
            if (Card.Status.CardRow.IsOnPlace() || Card.Status.CardRow.IsInCemetery() || Card.Status.CardRow == RowPosition.Banish) return;
            await ToCemetery();
            //***************************************
            //丢弃,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Lock(GameCard source = null)//锁定(未测试)
        {
            if (Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status.IsLock = !Card.Status.IsLock;
            await Game.ShowSetCard(Card);
            //***************************************
            //锁定,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Transform(GameCard To, GameCard source = null)//变为(未测试)
        {
            if (Card.Status.CardRow == RowPosition.Banish) return;
            Card.Status = To.Status;
            Card.Effect = To.Effect;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            //***************************************
            //变为,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Resurrect(CardLocation location, GameCard source = null)//复活(未测试)
        {
            if (Card.Status.CardRow == RowPosition.Banish && !Card.Status.CardRow.IsInCemetery()) return;
            var cardCemetery = Card.PlayerIndex;
            await Game.ShowCardMove(location, Card, true);
            await Game.SetPointInfo();
            await Game.SetCemeteryInfo(cardCemetery);
            //***************************************
            //变为,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Charm(GameCard source = null)//被魅惑
        {
            if (!Card.Status.CardRow.IsOnPlace()) return;
            if (Game.RowToList(Game.AnotherPlayer(Card.PlayerIndex), Card.Status.CardRow).Count >= 9) return;
            await Game.ShowCardOn(Card);
            await Game.ShowCardMove
            (
                new CardLocation()
                {
                    RowPosition = Card.Status.CardRow.RowMirror(),
                    CardIndex = Game.RowToList(Game.AnotherPlayer(Card.PlayerIndex), Card.Status.CardRow).Count
                },
                Card,
                true
            );
            await Task.Delay(400);
            await CardDown(true);
        }
        public virtual async Task Drain(int num, GameCard taget)//汲食
        {
            if (Card.Status.CardRow == RowPosition.Banish && Card.Status.CardRow.IsInCemetery()) return;
            var tagetNum = taget.Status.Strength + taget.Status.HealthStatus;
            num = num > tagetNum ? tagetNum : num;
            await taget.Effect.Damage(num, Card, BulletType.RedLight, true);//造成穿透伤害
            await Boost(num, Card);
            //***************************************
            //伏击,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
        public virtual async Task Ambush()//伏击
        {
            if (!Card.Status.CardRow.IsOnPlace() && !Card.Status.Conceal) return;
            Card.Status.Conceal = false;
            await Game.ShowSetCard(Card);
            await Game.ShowCardOn(Card);
            await Task.Delay(200);
            //***************************************
            //伏击,应该触发对应事件<暂未定义,待补充>
            //***************************************
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown(true);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDownEffect();
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
                await Game.ShowCardBreakEffect(taget, CardBreakEffectType.Consume);
                await taget.Effect.ToCemetery(false);
            }
            else
            {
                await taget.Effect.ToCemetery();
            }
            await Boost(num);
            await Task.Delay(500);
            //***************************************
            //吞噬,应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
    }
}