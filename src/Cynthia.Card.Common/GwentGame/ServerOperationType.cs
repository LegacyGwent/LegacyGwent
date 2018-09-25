namespace Cynthia.Card
{
    public enum ServerOperationType
    {
        GameStart,//游戏开始发送初始信息(暂时不需要更改)
        GetDragOrPass,//告诉玩家回合开始[需要做逻辑与功能上的调整]
        RoundEnd,//告诉玩家回合结束(暂时没用)
        MyCardEffectEnd,//告诉玩家我方卡牌效果结束 <感觉有问题>
        EnemyCardEffectEnd,//告诉玩家敌方卡牌效果结束<感觉有问题>
        EnemyCardDrag,//告诉玩家敌方拖拽卡牌<感觉有问题>
        SelectPlaceCard,//选择场上一些单位[等待完成]
        SelectAnotherCard,//选择一些其他卡[等待完成]
        GameEnd,//告诉玩家游戏结束,发送游戏结束信息(应该已经不用更改)
        EndOfBigRound,//结束小局(暂时没用)
        CardsToCemetery,//一些卡牌进入墓地 <感觉有问题>
        //-----------------------------------------------------------
        RemindYouRoundStart,//播放提示你的回合开始的动画
        //-----------------------------------------------------------
        //应该不需要改了
        BigRoundShowPoint,
        BigRoundSetMessage,
        BigRoundShowClose,
        //-----------------------------------------------------------
        //不需要改
        MulliganStart,//调度开始
        MulliganData,//调度数据(将第几张卡换成某一张)int gamecard
        GetMulliganInfo,//获取调度数据
        MulliganEnd,//调度结束
        //-----------------------------------------------------------
        //GetCardFrom,//从一个地方获取卡牌
        //SetCardTo,//将一张牌移动到另一个位置
        //(已替代上面两个)
        CardMove,//卡牌移动
        CardOn,//卡牌抬起
        CardDown,//卡牌落下
        //-----------------------------------------------------------
        //应该不需要大改
        SetMulliganInfo,//更新调度信息
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