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

        }

        //-----------------------------------------------------------
        //特殊卡的单卡效果
        public virtual void CardUse()
        {

        }

        //-----------------------------------------------------------
        //单位卡的单卡效果
        public virtual void Strengthen(int num)//强化
        {
            Card.CardStatus.Strength += num;
        }
        public virtual void Weaken(int num)//削弱
        {
            Card.CardStatus.Strength -= num;
            if (Card.CardStatus.Strength < 0)
            {
                Card.CardStatus.Strength = 0;
                Banish();
            }
        }
        public virtual void Boost(int num)//增益
        {
            Card.CardStatus.HealthStatus += num;
        }
        public virtual void Damage(int num)//伤害
        {
            Card.CardStatus.HealthStatus -= num;
            if (Card.CardStatus.HealthStatus + Card.CardStatus.Strength < 0)
            {
                Card.CardStatus.HealthStatus = -Card.CardStatus.Strength;
                ToCemetery();
            }
        }
        public virtual void Reset()//重置
        {
            Card.CardStatus.HealthStatus = 0;
        }
        public virtual void Heal()//治愈
        {
            if (Card.CardStatus.HealthStatus < 0)
                Card.CardStatus.HealthStatus = 0;
        }
    }
}