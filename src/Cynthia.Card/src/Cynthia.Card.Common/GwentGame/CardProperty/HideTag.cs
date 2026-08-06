namespace Cynthia.Card
{
    public enum HideTag : byte//卡牌隐藏的标签
    {
        Deathwish,  //遗愿
        Geralt,     //杰洛特
        Yennefer,   //叶奈法
        Triss,      //特莉丝
        Zoltan,     //卓尔坦
        Rule,       //规则卡：组卡时存在，开局后进入规则区并订阅事件
    }
}
