namespace Cynthia.Card
{
    public class CardEffect
    {
        public CardType Type { get; set; }//表示宿主是法术还是单位
        public GameCard Card { get; set; }//宿主
        public IGwentServerGame Game { get; set; }//游戏本体
        //-----------------------------------------------------------
        //公共效果
        public virtual void ToCemetery() { }//进入墓地

        //-----------------------------------------------------------
        //特殊卡的效果
        public virtual void CardUse()
        {
            ToCemetery();
        }

        //-----------------------------------------------------------
        //单位卡的效果
    }
}