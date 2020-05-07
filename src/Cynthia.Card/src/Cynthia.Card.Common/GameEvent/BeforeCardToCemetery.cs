namespace Cynthia.Card
{
    //卡牌进入墓地前
    public class BeforeCardToCemetery : Event
    {
        public GameCard Target { get; set; }

        public CardLocation DeathLocation { get; set; }

        public bool isRoundEnd { get; set; }
        public BeforeCardToCemetery(GameCard target, CardLocation deathLocation, bool IsRoundEnd=false)
        {
            Target = target;
            DeathLocation = deathLocation;
            isRoundEnd = IsRoundEnd;
        }
    }
}