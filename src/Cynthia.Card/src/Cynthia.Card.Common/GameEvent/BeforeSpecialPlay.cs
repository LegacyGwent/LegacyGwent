namespace Cynthia.Card
{
    //特殊卡被打出之前
    public class BeforeSpecialPlay : Event
    {
        public GameCard Target { get; set; }

        public bool IsUse { get; set; } = true;

        public BeforeSpecialPlay(GameCard target)
        {
            Target = target;
        }
    }
}