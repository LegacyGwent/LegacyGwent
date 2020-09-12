using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using Assets.Script.Localization.Serializables;
using UnityEngine;
using UnityEngine.Events;

namespace Assets.Script.Localization
{
    class LanguageManagerJson : ITranslator
    {
        private IDictionary<string, string> Texts = new Dictionary<string, string>();
        private IDictionary<string, CardLocale> CardTexts = new Dictionary<string, CardLocale>();
        public LanguageFileHandler FileHandler { get; } = new LanguageFileHandler();

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

        public LanguageManagerJson()
        {
            OnInit += () => {
                _languages = FileHandler.LoadLanguagesConfig();
                GameLanguage = PlayerPrefs.GetInt("Language", 0);
            };
            Initialize();
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