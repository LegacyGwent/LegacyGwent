namespace Cynthia.Card
{
    [CardEffectId("23020")]//戴维娜符文石
    public class DevenaRunestone : FactionRunestoneEffect
    {//落后时生成1个偶数铜色怪兽单位；领先时生成1个奇数铜色怪兽单位；平局不生效。
        public DevenaRunestone(GameCard card) : base(card) { }
        protected override Faction RunestoneFaction => Faction.Monsters;
    }
}
