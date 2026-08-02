namespace Cynthia.Card
{
    [CardEffectId("53018")]//莫拉纳符文石
    public class MoranaRunestone : FactionRunestoneEffect
    {//落后时生成1个偶数铜色松鼠党单位；领先时生成1个奇数铜色松鼠党单位；平局不生效。
        public MoranaRunestone(GameCard card) : base(card) { }
        protected override Faction RunestoneFaction => Faction.ScoiaTael;
    }
}
