namespace Cynthia.Card
{
    public static class RowStatusExtensions
    {
        public static bool IsHazard(this RowStatus rowStatus)
        {
            switch (rowStatus)
            {
                case RowStatus.KorathiHeatwave://科拉兹热浪
                case RowStatus.RaghNarRoog://终末之战
                case RowStatus.SkelligeStorm://史凯利杰风暴
                case RowStatus.DragonDream://龙之梦
                case RowStatus.BitingFrost://冰霜
                case RowStatus.ImpenetrableFog://浓雾
                case RowStatus.TorrentialRain://雨
                case RowStatus.PitTrap://坑陷
                case RowStatus.BloodMoon://血月
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsBoon(this RowStatus rowStatus)
        {
            switch (rowStatus)
            {
                case RowStatus.GoldenFroth://黄金酒沫
                case RowStatus.FullMoon://满月
                    return true;
                default:
                    return false;
            }
        }
    }
}