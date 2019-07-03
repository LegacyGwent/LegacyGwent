using System.Collections.Generic;

namespace Cynthia.Card
{
    public interface IHasEffects
    {
        EffectSet Effects { get; }
    }
}