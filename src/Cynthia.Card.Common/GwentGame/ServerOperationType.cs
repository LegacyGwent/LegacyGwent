namespace Cynthia.Card
{
    public enum ServerOperationType
    {
        GameStart,//游戏开始发送初始信息
        GetDragOrPass,//告诉玩家回合开始
        RoundEnd,//告诉玩家回合结束
        MyCardEffectEnd,//告诉玩家我方卡牌效果结束
        EnemyCardEffectEnd,//告诉玩家敌方卡牌效果结束
        EnemyCardDrag,//告诉玩家敌方拖拽卡牌
        SelectPlaceCard,//选择场上一些单位
        SelectAnotherCard,//选择一些其他卡
        GameEnd,//告诉玩家游戏结束,发送游戏结束信息
        EndOfBigRound,
        CardsToCemetery,
        //-----------------------------------------------------------
        SetCoinInfo,//更新硬币
        SetAllInfo, //更新所有信息
        SetGameInfo,//更新非卡牌相关信息
        SetCardsInfo,//更新卡牌相关信息
        SetPointInfo,//更新点数相关信息
        SetCountInfo,//更新数量方面信息
        SetPassInfo,//更新Pass方面信息
        SetWinCountInfo,//更新胜场方面信息
        SetMyCemetery,//更新我方墓地
        SetEnemyCemetery,//更新敌方墓地
        SetNameInfo,//更新名称方面信息(虽然感觉没什么意义)
    }
}