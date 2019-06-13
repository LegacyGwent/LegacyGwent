namespace Cynthia.Card
{
    public static class SelectModeTypeExtensions
    {
        public static bool IsHaveHand(this SelectModeType type)
        {
            switch (type)
            {
                case SelectModeType.All:
                    return true;
                case SelectModeType.AllHand:
                    return true;
                case SelectModeType.My:
                    return true;
                case SelectModeType.MyHand:
                    return true;
                case SelectModeType.Enemy:
                    return true;
                case SelectModeType.EnemyHand:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsHaveMy(this SelectModeType type)
        {
            switch (type)
            {
                case SelectModeType.All:
                    return true;
                case SelectModeType.AllHand:
                    return true;
                case SelectModeType.My:
                    return true;
                case SelectModeType.MyHand:
                    return true;
                case SelectModeType.AllRow:
                    return true;
                case SelectModeType.MyRow:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsHaveEnemy(this SelectModeType type)
        {
            switch (type)
            {
                case SelectModeType.All:
                    return true;
                case SelectModeType.AllHand:
                    return true;
                case SelectModeType.Enemy:
                    return true;
                case SelectModeType.EnemyHand:
                    return true;
                case SelectModeType.AllRow:
                    return true;
                case SelectModeType.EnemyRow:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsHaveRow(this SelectModeType type)
        {
            switch (type)
            {
                case SelectModeType.All:
                    return true;
                case SelectModeType.AllRow:
                    return true;
                case SelectModeType.My:
                    return true;
                case SelectModeType.MyRow:
                    return true;
                case SelectModeType.Enemy:
                    return true;
                case SelectModeType.EnemyRow:
                    return true;
                default:
                    return false;
            }
        }
    }
}