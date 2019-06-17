namespace Cynthia.Card
{
    //玩家抽牌后
    public class AfterPlayerDraw : Event
    {
        public GameCard DrawCard { get; set; }

        public GameCard Source { get; set; }

        public int PlayerIndex { get; set; }

        public AfterPlayerDraw(int playerIndex, GameCard drawCard, GameCard source)
        {
            DrawCard = drawCard;
            Source = source;
            PlayerIndex = playerIndex;
        }
    }
}