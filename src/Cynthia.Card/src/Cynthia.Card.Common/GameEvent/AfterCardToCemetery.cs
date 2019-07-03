namespace Cynthia.Card
{
    //卡牌进入墓地后
    public class AfterCardToCemetery : Event
    {
        public GameCard Target { get; set; }

        public CardLocation DeathLocation { get; set; }
        public AfterCardToCemetery(GameCard target, CardLocation deathLocation)
        {
            Target = target;
            DeathLocation = deathLocation;
        }
    }
}