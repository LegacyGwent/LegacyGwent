namespace Cynthia.Card
{
    //卡牌落下后
    public class AfterUnitDown : Event
    {
        public GameCard Target { get; set; }

        public AfterUnitDown(GameCard target)
        {
            Target = target;
        }
    }
}