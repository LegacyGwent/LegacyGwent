namespace Cynthia.Card
{
    public enum GetRangeType//对一个范围卡牌的选取规则
    {
        CenterAll,//全部
        HollowAll,//不包含选中卡的全部
        CenterLeft,//包含选中卡与其左侧的卡
        CenterRight,//包含选中卡与其右侧的卡
        HollowLeft,//选中卡左侧的卡牌
        HollowRight,//选中卡右侧的卡牌
    }
}