public enum GameOperationType //当前需要用户进行什么操作?
{
    None,           //不需要操作
    GetPassOrGrag,  //拖动,或者pass
    SelectCards,    //选择一或者多张卡牌
    SelectRow,      //选择某一排

    PlayCard,   //要求放置一个悬牌
}
