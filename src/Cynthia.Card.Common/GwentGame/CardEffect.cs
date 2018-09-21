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
        public virtual void ToCemetery()//进入墓地触发
        {
            if (Card.CardStatus.IsDoomed)//如果是佚亡,放逐
            {
                Banish();
            }
        }
        public virtual void Banish()//放逐
        {
            //需要补充
        }

        //-----------------------------------------------------------
        //特殊卡的单卡效果
        public virtual void CardUse()//使用
        {
            CardUseStart();
            CardUseEffect();
            CardUseEnd();
        }
        public virtual void CardUseStart()//使用效果
        {

        }
        public virtual void CardUseEffect()//使用效果
        {

        }
        public virtual void CardUseEnd()//使用结束
        {
            ToCemetery();
        }

        //-----------------------------------------------------------
        //单位卡的单卡所受效果
        public virtual void BigRoundEnd()//小局结束
        {
            //if (Card.CardStatus.Location.RowPosition)
        }
        public virtual void Strengthen(int num, CardLocation taget = null)//强化
        {
            Card.CardStatus.Strength += num;
        }
        public virtual void Weaken(int num, CardLocation taget = null)//削弱
        {
            Card.CardStatus.Strength -= num;
            if (Card.CardStatus.Strength < 0)
            {
                Card.CardStatus.Strength = 0;
                Banish();
            }
        }
        public virtual void Boost(int num, CardLocation taget = null)//增益
        {
            Card.CardStatus.HealthStatus += num;
        }
        public virtual void Damage(int num, CardLocation taget = null)//伤害
        {
            Card.CardStatus.HealthStatus -= num;
            if (Card.CardStatus.HealthStatus + Card.CardStatus.Strength < 0)
            {
                Card.CardStatus.HealthStatus = -Card.CardStatus.Strength;
                ToCemetery();
            }
        }
        public virtual void Reset(CardLocation taget = null)//重置
        {
            Card.CardStatus.HealthStatus = 0;
        }
        public virtual void Heal(CardLocation taget = null)//治愈
        {
            if (Card.CardStatus.HealthStatus < 0)
                Card.CardStatus.HealthStatus = 0;
        }
    }
}