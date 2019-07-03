using System.Collections.Generic;

namespace LegacyGwent
{
    public interface IHasEffects
    {
        EffectSet Effects { get; }
    }
}