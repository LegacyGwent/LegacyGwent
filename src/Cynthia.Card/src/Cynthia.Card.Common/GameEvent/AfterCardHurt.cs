namespace Cynthia.Card
{
    //发生"受伤"后
    public class AfterCardHurt : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }
        public int Num { get; set; }
        public DamageType DamageType { get; set; }

        public AfterCardHurt(GameCard target, int num, GameCard source, DamageType damageType)
        {
            Target = target;
            Num = num;
            Source = source;
            DamageType = damageType;
        }
    }
}