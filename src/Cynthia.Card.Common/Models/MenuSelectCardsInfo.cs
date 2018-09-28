using System.Collections.Generic;

namespace Cynthia.Card
{
    public class MenuSelectCardInfo//在菜单中选择一些卡牌
    {
        public IList<CardStatus> SelectList;//从这些卡中选择
        public int SelectCount;//选择多少张?
        public bool IsCanOver = true;//是否可以提前结束
        public string OverMessage = "确认";//提前结束的按钮内容
        public string Title = "选择一张卡牌";//标题
        /* 
        选择生成一张牌
        选择打出一张牌
        选择一张牌
        选择将一张牌放回牌组
        选择抽取一张牌
        选择揭示一张牌*/
    }
}