namespace Cynthia.Card
{
    //发生"受伤"后
    public class BeforeCardHurt : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }
        public bool IsCancel { get; set; } = false;
        public DamageType DamageType { get; set; }

        public BeforeCardHurt(GameCard target, int num, GameCard source, DamageType damageType)
        {
            Target = target;
            Num = num;
            Source = source;
            DamageType = damageType;
        }
    }
}