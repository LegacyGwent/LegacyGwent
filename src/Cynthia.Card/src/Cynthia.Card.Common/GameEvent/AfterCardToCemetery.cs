namespace Cynthia.Card
{
    //卡牌进入墓地后
    public class AfterCardToCemetery : Event
    {
        public GameCard Target { get; set; }

        public CardLocation DeathLocation { get; set; }

        public bool isRoundEnd { get; set; }
        public AfterCardToCemetery(GameCard target, CardLocation deathLocation, bool IsRoundEnd=false)
        {
            Target = target;
            DeathLocation = deathLocation;
            isRoundEnd = IsRoundEnd;
        }
    }
}