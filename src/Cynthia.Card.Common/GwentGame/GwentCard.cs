namespace Cynthia.Card
{
    //一个卡牌类型
    public class GwentCard : ModelBase
    {
        //包含标签  例如:士兵,精灵 (多选)
        public string[] Categories { get; set; }
        //所属势力  例如:松鼠党
        public string Faction { get; set; }
        //卡牌描述  例如:管他泰莫利亚人还是瑞达尼亚人，杀无赦。
        public Flavor Flavor { get; set; }
        //卡牌品质  例如:铜卡
        public string Group { get; set; }
        //能力描述  例如:使所有“精灵”友军获得1点增益。 每次被交换时，再次触发此能力。
        public string Info { get; set; }
        //卡牌名称  例如:维里赫德旅先锋
        public string Name { get; set; }
        //战力数值  例如:6
        public int Strength { get; set; }
        //可下排位 例如:任意排 (多选)
        public string[] Positions { get; set; }
    }
}