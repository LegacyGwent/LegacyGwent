namespace Cynthia.Card
{
    //卡牌死亡后
    public class AfterCardDeath : Event
    {
        public GameCard Target { get; set; }

        public CardLocation DeathLocation { get; set; }
        public AfterCardDeath(GameCard target, CardLocation deathLocation)
        {
            Target = target;
            DeathLocation = deathLocation;
        }
    }
}