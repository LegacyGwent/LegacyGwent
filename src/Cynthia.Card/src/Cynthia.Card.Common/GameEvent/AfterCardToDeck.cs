namespace Cynthia.Card
{
    // A card has returned to a player's deck from another zone.
    public class AfterCardToDeck : Event
    {
        public GameCard Target { get; set; }

        public AfterCardToDeck(GameCard target)
        {
            Target = target;
        }
    }
}
