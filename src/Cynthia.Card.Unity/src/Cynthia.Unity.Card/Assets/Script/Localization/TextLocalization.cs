using Assets.Script.ResourceManagement;
using System;
using System.Collections.Generic;
using Cynthia.Card.Common.Models.Localization;
using UnityEngine;

namespace Assets.Script.Localization
{
    class TextLocalization
    {
        public const string PreferenceKey = "DiyAi.TextLanguage";

        private IList<ConfigEntry> _languages;
        public ConfigEntry ChosenLanguage { get; private set; }
        public int ChosenLanguageIndex => _languages.IndexOf(ChosenLanguage);

        private ILocalizationResourceHandler _resourceHandler;
        public ILocalizationResourceHandler ResourceHandler
        {
            get => _resourceHandler;
            set
            {
                _resourceHandler = value;
                _languages = _resourceHandler.LoadConfiguration();
                ChooseLanguage(PlayerPrefs.GetInt(PreferenceKey, GetChineseLanguageIndex()));
            }
        }

        public IDictionary<string, string> Texts;
        public IDictionary<string, CardLocale> CardTexts;

        public TextLocalization()
        {
            ResourceHandler = new LocalizationResourceHandler("Locales");
        }
        private int GetChineseLanguageIndex()
        {
            for (var index = 0; index < _languages.Count; index++)
            {
                if (string.Equals(_languages[index].Filename, "cn", StringComparison.OrdinalIgnoreCase))
                    return index;
            }
            return 0;
        }
        public int ChooseLanguage(int index)
        {
            index %= _languages.Count;
            ChosenLanguage = _languages[index];

            var loadedLocale = ResourceHandler.LoadResource(ChosenLanguage.Filename);
            Texts = loadedLocale.MenuLocales;
            CardTexts = loadedLocale.CardLocales;

            return index;
        }
        public bool IsContainsKey(string id)
        {
            return Texts.ContainsKey(id) ? true : false;
        }
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
