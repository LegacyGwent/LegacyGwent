namespace Cynthia.Card
{
    //天气落下后
    public class AfterWeatherApply : Event
    {
        public int PlayerIndex { get; set; }
        public RowPosition Row { get; set; }
        public RowStatus Type { get; set; }
        public AfterWeatherApply(int playerIndex, RowPosition row, RowStatus type)
        {
            PlayerIndex = playerIndex;
            Row = row;
            Type = type;
        }
    }
}