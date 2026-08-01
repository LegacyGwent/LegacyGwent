using System.Collections.Generic;
using System.IO;
using Cynthia.Card.Common.Models.Localization;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Cynthia.Card.Server.Services.GwentGameService
{
    public class GwentLocalizationService
    {
        private readonly string _gameLocales;
        private readonly IReadOnlyDictionary<string, GameLocale> _locales;

        public GwentLocalizationService()
        {
            var config = new List<ConfigEntry>();
            var currentDirectory = Directory.GetCurrentDirectory();
            using (var stream = new StreamReader($"{currentDirectory}/Locales/config.json"))
            {
                var serializedConfig = stream.ReadToEnd();
                config = JsonConvert.DeserializeObject<List<ConfigEntry>>(serializedConfig);
            }

            var loadedLocales = new List<GameLocale>();
            foreach (var locale in config)
            {
                var filePath = $"{currentDirectory}/Locales/{locale.Filename}.json";
                var loadedLocale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(filePath));
                if (string.Equals(locale.Filename, "cn", StringComparison.OrdinalIgnoreCase))
                {
                    ApplyChineseCardRules(loadedLocale);
                }
                loadedLocales.Add(loadedLocale);
            }

            _locales = loadedLocales
                .Where(locale => locale?.Info != null && !string.IsNullOrWhiteSpace(locale.Info.Filename))
                .GroupBy(locale => locale.Info.Filename, StringComparer.OrdinalIgnoreCase)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.OrdinalIgnoreCase);
            _gameLocales = JsonConvert.SerializeObject(loadedLocales);
        }

        public static void ApplyChineseCardRules(GameLocale gameLocale)
        {
            if (gameLocale == null)
            {
                throw new ArgumentNullException(nameof(gameLocale));
            }

            if (gameLocale.CardLocales == null)
            {
                gameLocale.CardLocales = new Dictionary<string, CardLocale>(StringComparer.Ordinal);
            }

            foreach (var card in GwentMap.CardMap)
            {
                if (!gameLocale.CardLocales.TryGetValue(card.Key, out var cardLocale)
                    || cardLocale == null)
                {
                    cardLocale = new CardLocale();
                    gameLocale.CardLocales[card.Key] = cardLocale;
                }

                // GwentMap is the gameplay source of truth. The locale keeps
                // flavor text, but cannot override the active Chinese rules.
                cardLocale.Name = card.Value.Name;
                cardLocale.Info = card.Value.Info;
            }
        }

        public string GetGameLocales()
        {
            return _gameLocales;
        }

        public string GetMenuText(string locale, string key)
        {
            if (string.IsNullOrWhiteSpace(key)
                || !_locales.TryGetValue(locale ?? string.Empty, out var gameLocale)
                || gameLocale.MenuLocales == null
                || !gameLocale.MenuLocales.TryGetValue(key, out var value)
                || string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            return value;
        }

        public string GetCardName(string locale, string cardId)
        {
            if (string.IsNullOrWhiteSpace(cardId)
                || !_locales.TryGetValue(locale ?? string.Empty, out var gameLocale)
                || gameLocale.CardLocales == null
                || !gameLocale.CardLocales.TryGetValue(cardId, out var cardLocale)
                || string.IsNullOrWhiteSpace(cardLocale?.Name))
            {
                return null;
            }

            return cardLocale.Name;
        }
    }
}
