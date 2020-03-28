using System.Linq;
using System.Reflection;
using System.Collections.Concurrent;
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace Cynthia.Card.Server
{
    public class GwentCardDataService
    {
        private IDictionary<string, Type> _idDictionary;

        private string _cardMapData;

        public GwentCardDataService()
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

            var cardMapString = JsonConvert.SerializeObject(GwentMap.CardMap);
            _cardMapData = cardMapString;
        }

        public string GetCardMap()
        {
            return _cardMapData;
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