namespace Cynthia.Card
{
    //卡牌护甲被破坏后
    public class AfterCardArmorBreak : Event
    {
        public GameCard Target { get; set; }
        public GameCard Source { get; set; }

        public AfterCardArmorBreak(GameCard target, GameCard source)
        {
            Target = target;
            Source = source;
        }
    }
}