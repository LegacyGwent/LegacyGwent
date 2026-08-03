namespace Cynthia.Card
{
    // The unit has visually landed after its entry action, before global landing reactions.
    public class AfterUnitLanded : Event
    {
        public GameCard Target { get; }

        public AfterUnitLanded(GameCard target)
        {
            Target = target;
        }
    }
}
