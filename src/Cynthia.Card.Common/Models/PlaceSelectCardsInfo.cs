namespace Cynthia.Card
{
    public class PlaceSelectCardsInfo
    {
        public GameCardsPart CanSelect;//那些卡牌可以选择
        public CardLocation SelectCard;//什么卡牌进行选择
        public int Range = 0;//向两边延伸的范围
        public int SelectCount = 0;//选择数量为
    }
}