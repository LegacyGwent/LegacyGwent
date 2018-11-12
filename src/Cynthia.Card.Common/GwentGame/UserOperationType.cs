namespace Cynthia.Card
{
    public enum UserOperationType : byte
    {
        None,//超时操作
        OK,//回执
        RoundOperate,//回合进行的操作(放置)
        MulliganInfo,//调度的相关信息
        //----------------------------------
        //最终
        SelectMenuCardsInfo,//选择了一些指定的卡牌√(实现)
        SelectPlaceCardsInfo,//选择了一些场上的卡牌
        SelectRowInfo,//选择了一排
        PlayCardInfo,//选择将一个卡打到了某个位置
    }
}