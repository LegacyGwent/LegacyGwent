using System.Threading.Tasks;

namespace Cynthia.Card
{
    public class CardEffect
    {
        public GameCard Card { get; set; }//宿主
        public IGwentServerGame Game { get; set; }//游戏本体

        public CardEffect(IGwentServerGame game, GameCard card)
        {
            Game = game;
            Card = card;
        }

        //-----------------------------------------------------------
        //公共效果
        public virtual async Task ToCemetery()//进入墓地触发
        {
            await Game.ShowCardMove(new CardLocation() { RowPosition = RowPosition.MyCemetery, CardIndex = 0 }, Card);
            await Task.Delay(400);
            if (Card.Status.IsDoomed)//如果是佚亡,放逐
            {
                await Banish();
            }
            await Game.SetCemeteryInfo(Card.PlayerIndex);
            await Game.SetPointInfo();
        }
        public virtual async Task Banish()//放逐
        {
            //需要补充
            await Task.CompletedTask;
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
            await CardPlayStart(location);
            var count = await CardPlayEffect();
            if (Card.Status.CardRow.IsOnPlace())
                await CardDown();
            await PlayStayCard(count);
            if (Card.Status.CardRow.IsOnPlace())
                await CardDownEffect();
        }
        public virtual async Task CardPlayStart(CardLocation location)//先是移动到目标地点
        {
            await Game.ShowCardOn(Card);
            await Game.ShowCardMove(location, Card);
            await Task.Delay(300);
        }
        public virtual async Task PlayStayCard(int count)
        {
            await Task.CompletedTask;
        }
        public virtual async Task<int> CardPlayEffect()
        {
            await Damage(5);
            //休战,双方各抽一张牌,并将敌方抽到的牌揭示
            /*
            if (!Game.IsPlayersPass[Game.Player1Index] && !Game.IsPlayersPass[Game.Player2Index])
            {
                await Game.DrawCard(1, 1);
                var enemyCard = Game.PlayersHandCard[Game.AnotherPlayer(Card.PlayerIndex)][0];
                await enemyCard.Effect.Reveal();
            }*/
            await Task.Delay(200);
            return 0;
        }
        public virtual async Task CardDown()
        {
            await Game.ShowCardDown(Card);
            await Game.SetPointInfo();
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
        public virtual async Task Weaken(int num, GameCard taget = null)//削弱
        {
            Card.Status.Strength -= num;
            if (Card.Status.Strength < 0)
            {
                Card.Status.Strength = 0;
                await Banish();
            }
        }
        public virtual async Task Boost(int num, GameCard source = null)//增益
        {
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
            //最高承受伤害,如果穿透的话,不考虑护甲
            var bear = (Card.Status.Strength + (isPenetrate ? 0 : Card.Status.Armor) + Card.Status.HealthStatus);
            Card.Status.HealthStatus -= num;
            if (Card.Status.HealthStatus + Card.Status.Strength < 0)
            {
                Card.Status.HealthStatus = -Card.Status.Strength;
                await ToCemetery();
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
            await Task.CompletedTask;
        }
    }
}