using Cynthia.Card.Server;

namespace Cynthia.Card
{
    public abstract class CardEffect
    {
        public GwentServerGame Game { get; set; }
        public GameCard Card { get; set; }
        public CardType Type { get; set; }
        public CardEffect(GwentServerGame game)
        {
            Game = game;
        }
        //--------------------------------------------------------
        //法术卡的方法
        public void UseCard() { }

        //--------------------------------------------------------
        //单位卡
        public void PlayCard() { }
    }
}