namespace Cynthia.Card
{
    /// <summary>
    /// Raised after a concealed ambush has actually entered the battlefield.
    /// Ambushes intentionally skip the ordinary AfterUnitDown pipeline until
    /// they reveal, so effects that count face-down ambushes use this event.
    /// </summary>
    public class AfterAmbushDown : Event
    {
        public GameCard Target { get; }
        public bool IsFromHand { get; }
        public bool IsPlayed { get; }

        public AfterAmbushDown(GameCard target, bool isFromHand, bool isPlayed = false)
        {
            Target = target;
            IsFromHand = isFromHand;
            IsPlayed = isPlayed;
        }
    }
}
