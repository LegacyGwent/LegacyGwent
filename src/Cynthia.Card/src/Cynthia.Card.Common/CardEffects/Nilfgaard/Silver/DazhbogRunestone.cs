namespace Cynthia.Card
{
    [CardEffectId("33019")]//达兹伯格符文石
    public class DazhbogRunestone : FactionRunestoneEffect
    {//落后时生成1个偶数铜色尼弗迦德单位；领先时生成1个奇数铜色尼弗迦德单位；平局不生效。
        public DazhbogRunestone(GameCard card) : base(card) { }
        protected override Faction RunestoneFaction => Faction.Nilfgaard;
    }
}
