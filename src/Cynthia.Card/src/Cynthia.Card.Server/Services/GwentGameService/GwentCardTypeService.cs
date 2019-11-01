using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;

namespace Cynthia.Card.Server
{
    public class GwentCardTypeService
    {
        private IDictionary<string, Type> _idDictionary;

        public GwentCardTypeService()
        {
            _idDictionary = new ConcurrentDictionary<string, Type>();
            var assembly = typeof(CardEffect).Assembly;
            var cardEffects = assembly.GetTypes().Where(x => x.GetCustomAttributes(true).Any(x => x.GetType() == typeof(CardEffectIdAttribute)));
            foreach (var cardEffect in cardEffects)
            {
                foreach (CardEffectIdAttribute cardId in cardEffect.GetCustomAttributes(typeof(CardEffectIdAttribute), true))
                {
                    if (_idDictionary.ContainsKey(cardId.Id)) continue;
                    _idDictionary.Add(cardId.Id, cardEffect);
                }
            }
        }

        public Type GetType(string effectId)
        {
            if (_idDictionary.ContainsKey(effectId))
            {
                return _idDictionary[effectId];
            }
            return typeof(NoneEffect);
        }

        public CardEffect CreateInstance(string effectId, GameCard targetCard)
            => (CardEffect)Activator.CreateInstance(GetType(effectId), targetCard);
    }
}