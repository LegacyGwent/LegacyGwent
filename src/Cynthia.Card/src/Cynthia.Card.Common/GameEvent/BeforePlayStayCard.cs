namespace Cynthia.Card
{
    public class BeforePlayStayCard : Event
    {
        public GameCard Target { get; set; }
        public int PlayCount { get; set; }

        public BeforePlayStayCard(GameCard target, int playCount)
        {
            Target = target;
            PlayCount = playCount;
        }
    }
}