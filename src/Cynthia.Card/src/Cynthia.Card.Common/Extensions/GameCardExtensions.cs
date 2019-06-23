using System;
using System.Collections.Generic;
using System.Linq;
using Alsein.Extensions.Extensions;
using Newtonsoft.Json;

namespace Cynthia.Card
{
    public static class GameCardExtensions
    {
        public static void AddEffects(this GameCard card, params string[] effectIds)
        {
            foreach (var effectId in effectIds)
            {
                card.Effects.Add(card.Game.CreateEffectInstance(effectId, card));
            }
        }
    }
}