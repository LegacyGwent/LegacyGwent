using Cynthia.Card.Server;

namespace Cynthia.Card
{
    public abstract class CardEffect
    {
        public GwentServerGame Game { get; set; }
        public GameCard Card { get; set; }
        public CardType Type { get; set; }
        public CardEffect(GwentServerGame game, GameCard card)
        {
            Game = game;
            Card = card;
        }
        public CardEffect(GwentServerGame game, string cardIndex)
        {
            Game = game;
            Card = new GameCard(cardIndex);
            Type = Card.Type;
        }
        //--------------------------------------------------------
        //法术卡的方法
        public void UseCard() { }

        //--------------------------------------------------------
        //单位卡
        public void PlayCard() { }
    }
}