namespace Cynthia.Card
{
    //卡牌被放逐后
    public class AfterCardBanish : Event
    {
        public GameCard Target { get; set; }

        public CardLocation BanishLocation { get; set; }
        public AfterCardBanish(GameCard target, CardLocation banishLocation)
        {
            Target = target;
            BanishLocation = banishLocation;
        }
    }
}