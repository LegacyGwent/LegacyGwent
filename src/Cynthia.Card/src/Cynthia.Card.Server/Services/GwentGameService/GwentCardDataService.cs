using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Cynthia.Card.Server
{
    public class GwentCardDataService
    {
        private IDictionary<string, Type> _idDictionary;

        private string _cardMapData;
        private string _avatarMapData; // stores the data for the avatar cosmetics
        private string _borderMapData; // stores the data for the border cosmetics
        private string _titleMapData; // stores the data for the titles cosmetics

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
            var avatarMapString = JsonConvert.SerializeObject(TrinketMap.AvatarMap);
            _avatarMapData = avatarMapString;
            var borderMapString = JsonConvert.SerializeObject(TrinketMap.BorderMap);
            _borderMapData = borderMapString;
            var titleMapString = JsonConvert.SerializeObject(TrinketMap.TitleMap);
            _titleMapData = titleMapString;
        }

        public string GetCardMap()
        {
            return _cardMapData;
        }
        public string GetAvatarMap()
        {
            return _avatarMapData;
        }
        public string GetBorderMap()
        {
            return _borderMapData;
        }
        public string GetTitleMap()
        {
            return _titleMapData;
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