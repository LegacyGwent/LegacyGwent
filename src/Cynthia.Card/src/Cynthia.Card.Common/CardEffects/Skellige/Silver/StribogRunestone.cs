namespace Cynthia.Card
{
    [CardEffectId("63018")]//史璀伯格符文石
    public class StribogRunestone : FactionRunestoneEffect
    {//落后时生成1个偶数铜色史凯利格单位；领先时生成1个奇数铜色史凯利格单位；平局不生效。
        public StribogRunestone(GameCard card) : base(card) { }
        protected override Faction RunestoneFaction => Faction.Skellige;
    }
}
