namespace Cynthia.Card
{
    [CardEffectId("43019")]//佐里亚符文石
    public class ZoriaRunestone : FactionRunestoneEffect
    {//落后时生成1个偶数铜色北方领域单位；领先时生成1个奇数铜色北方领域单位；平局不生效。
        public ZoriaRunestone(GameCard card) : base(card) { }
        protected override Faction RunestoneFaction => Faction.NorthernRealms;
    }
}
