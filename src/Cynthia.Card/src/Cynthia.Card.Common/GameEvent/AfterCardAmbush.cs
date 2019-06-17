namespace Cynthia.Card
{
    //伏击翻开后
    public class AfterCardAmbush : Event
    {
        public GameCard Target { get; set; }
        public AfterCardAmbush(GameCard target)
        {
            Target = target;
        }
    }
}