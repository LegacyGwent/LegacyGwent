using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
        public virtual async Task ToCemetery()//进入墓地触发
        {
            Card.Status.Armor = 0; //护甲归零
            Card.Status.HealthStatus = 0;//没有增益和受伤
            Card.Status.IsCardBack = false; //没有背面
            Card.Status.IsResilience = false;//没有坚韧
            Card.Status.IsShield = false; //没有昆恩
            Card.Status.IsSpying = false; //没有间谍
            Card.Status.Conceal = false;  //没有隐藏
            Card.Status.IsReveal = false; //没有揭示
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
            if (Card.Status.IsDoomed)//如果是佚亡,放逐
            {
                await Banish();
                return;
            }
            await Game.SetCemeteryInfo(Card.PlayerIndex);
            await Game.SetPointInfo();
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
            await Task.Delay(400);
            //群体发送事件
        }
        public virtual async Task CardUseEffect()//使用效果
        {
            await Task.CompletedTask;
        }
        public virtual async Task CardUseEnd()//使用结束
        {
            await ToCemetery();
        }

        //-----------------------------------------------------------
        //单位卡的单卡放置
        public virtual async Task Play(CardLocation location)//放置
        {
            var isSpying = await CardPlayStart(location);
            var count = await CardPlayEffect();
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
            await Task.CompletedTask;
        }
        public virtual async Task<int> CardPlayEffect()
        {
            if (!Game.IsPlayersPass[AnotherPlayer])//如果对方没有pass
            {
                await Boost(10);//增益自身10点
                var c = Game.PlayersDeck[AnotherPlayer].Where(x => x.Status.Group == Group.Copper);
                //检查对方卡组的铜色单位,如果有铜色单位的话
                if (c.Count() != 0)
                {
                    //将这张牌移动到对方的卡组顶端
                    Game.LogicCardMove
                    (
                        Game.PlayersDeck[AnotherPlayer], Game.PlayersDeck[AnotherPlayer].IndexOf(c.First()),
                        Game.PlayersDeck[AnotherPlayer], 0
                    );
                    //让对方抽一张卡
                    var drawCard = default(IList<GameCard>);
                    if (AnotherPlayer == Game.Player1Index)
                    {
                        (drawCard, _) = await Game.DrawCard(1, 0);
                    }
                    else
                    {
                        (_, drawCard) = await Game.DrawCard(0, 1);
                    }
                    await drawCard.Single().Effect.Reveal();
                }
            }
            await Task.Delay(200);
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
        public virtual async Task CardDownEffect()
        {
            await Task.CompletedTask;
        }
        //------------------------------------------------------------
        //单位卡的单卡所受效果
        public virtual async Task BigRoundEnd()//小局结束
        {
            //if (Card.CardStatus.Location.RowPosition)
            await Task.CompletedTask;
        }
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
        }
        public virtual async Task Weaken(int num, GameCard source = null)//削弱
        {
            if (num <= 0) return;
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
            await Game.ShowCardNumberChange(Card, num > showBear ? showBear : num, NumberType.White);
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
            if (num <= 0) return;
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
        }
        public virtual async Task Damage(int num, GameCard taget = null, bool isPenetrate = false)//伤害
        {
            if (num <= 0) return;
            //最高承受伤害,如果穿透的话,不考虑护甲
            var die = false;
            var isArmor = Card.Status.Armor > 0;
            var bear = (Card.Status.Strength + (isPenetrate ? 0 : Card.Status.Armor) + Card.Status.HealthStatus);
            if (num >= bear)
            {
                num = bear;//如果数值大于最高伤害的话,进行限制
                die = true;//死亡,不会触发任何效果
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
        public virtual async Task Reset(GameCard taget = null)//重置
        {
            Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
            if (Card.Status.Strength <= 0)
            {
                await Banish();
            }
        }
        public virtual async Task Heal(GameCard taget = null)//治愈
        {
            if (Card.Status.HealthStatus < 0)
                Card.Status.HealthStatus = 0;
            await Game.ShowSetCard(Card);
            await Game.SetPointInfo();
        }
        public virtual async Task Reveal()//揭示
        {
            //不在手上的话,或者已经被揭示,没有效果
            if (!Card.Status.CardRow.IsInHand() || Card.Status.IsReveal)
                return;
            Card.Status.IsReveal = true;
            await Game.ShowSetCard(Card);
        }
        public virtual async Task Spying()//间谍
        {
            Card.Status.IsSpying = !Card.Status.IsSpying;
            await Game.ShowSetCard(Card);
            //***************************************
            //打出了卡牌(间谍?),应该触发对应事件<暂未定义,待补充>
            //***************************************
        }
    }
}