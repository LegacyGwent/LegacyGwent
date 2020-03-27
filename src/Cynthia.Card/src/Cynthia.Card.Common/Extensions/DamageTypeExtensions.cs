namespace Cynthia.Card
{
    public static class DamageTypeExtensions
    {
        public static bool IsHazard(this DamageType DamageType)
        {
            switch (DamageType)
            {
                case DamageType.KorathiHeatwave://科拉兹热浪
                case DamageType.RaghNarRoog://终末之战
                case DamageType.SkelligeStorm://史凯利杰风暴
                case DamageType.DragonDream://龙之梦
                case DamageType.BitingFrost://冰霜
                case DamageType.ImpenetrableFog://浓雾
                case DamageType.TorrentialRain://雨
                case DamageType.PitTrap://坑陷
                case DamageType.BloodMoon://血月
                    return true;
                default:
                    return false;
            }
        }
    }
}