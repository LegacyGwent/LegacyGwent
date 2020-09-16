using Alsein.Extensions.LifetimeAnnotations;
using Assets.Script.Localization.Serializables;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Localization
{
    [Singleton]
    class LocalizationService
    {
        private IDictionary<string, string> Texts = new Dictionary<string, string>();
        private IDictionary<string, CardLocale> CardTexts = new Dictionary<string, CardLocale>();
        public LocalizationFileHandler FileHandler { get; } = new LocalizationFileHandler();

        private List<LocaleInfo> _languages;
        private LocaleInfo _currentLanguage;

        public int GameLanguage
        {
            get => _languages.IndexOf(_currentLanguage);
            set
            {
                if (value >= _languages.Count || value < 0)
                {
                    value = 0;
                }
                _currentLanguage = _languages[value];
                var loadedLocale = FileHandler.LoadLocale(_currentLanguage.Filename);
                Texts = loadedLocale.MenuLocales;
                CardTexts = loadedLocale.CardLocales;
            }
        }
        public List<string> LanguageNames => _languages.Select(l => l.Name).ToList();
        public List<string> LanguageFilenames => _languages.Select(l => l.Filename).ToList();

        public LocalizationService()
        {
            OnInit += () =>
            {
                _languages = FileHandler.LoadLanguagesConfig();
                GameLanguage = PlayerPrefs.GetInt("Language", 0);
            };
        }
        public void Initialize()
        {
            OnInit?.Invoke();
        }

        public event Action OnInit;

        public string GetText(string id)
        {
            return Texts.ContainsKey(id) ? Texts[id] : id;
        }
        public string GetCardName(string cardId)
        {
            return CardTexts.ContainsKey(cardId) ? CardTexts[cardId].Name : $"{cardId}_Name";
        }
        public string GetCardInfo(string cardId)
        {
            return CardTexts.ContainsKey(cardId) ? CardTexts[cardId].Info : $"{cardId}_Info";
        }
        public string GetCardFlavor(string cardId)
        {
            return CardTexts.ContainsKey(cardId) ? CardTexts[cardId].Flavor : $"{cardId}_Flavor";
        }
    }
}