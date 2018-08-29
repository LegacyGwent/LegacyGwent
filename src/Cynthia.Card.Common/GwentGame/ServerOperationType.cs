namespace Cynthia.Card
{
    public enum ServerOperationType
    {
        GameStart,//游戏开始发送初始信息
        GameInfomation,//游戏中推送信息
        GetDragOrPass,//告诉玩家回合开始
        RoundEnd,//告诉玩家回合结束
        MyCardEffectEnd,//告诉玩家我方卡牌效果结束
        EnemyCardEffectEnd,//告诉玩家敌方卡牌效果结束
        EnemyCardDrag,//告诉玩家敌方拖拽卡牌
        SelectPlaceCard,//选择场上一些单位
        SelectAnotherCard,//选择一些其他卡
        GameEnd//告诉玩家游戏结束
    }
}