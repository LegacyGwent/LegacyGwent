namespace Cynthia.Card
{
    public static class RowPositionExtensions
    {
        public static bool IsOnPlace(this RowPosition row)
        {
            switch (row)
            {
                case RowPosition.MyRow1:
                    return true;
                case RowPosition.MyRow2:
                    return true;
                case RowPosition.MyRow3:
                    return true;
                case RowPosition.EnemyRow1:
                    return true;
                case RowPosition.EnemyRow2:
                    return true;
                case RowPosition.EnemyRow3:
                    return true;
                default:
                    return false;
            }
        }
    }
}