namespace Cynthia.Card
{
    //卡牌被放逐后
    public class BeforeCardBanish : Event
    {
        public GameCard Target { get; set; }

        public CardLocation BanishLocation { get; set; }
        public BeforeCardBanish(GameCard target, CardLocation banishLocation)
        {
            Target = target;
            BanishLocation = banishLocation;
        }
    }
}