using Assets.Script.ResourceManagement;
using System;
using System.Collections.Generic;
using Cynthia.Card.Common.Models.Localization;
using UnityEngine;
using System.Globalization;

namespace Assets.Script.Localization
{
    class TextLocalization
    {
        public const string PreferenceKey = "DiyAi.TextLanguage";
        public static event Action LanguageChanged;
        public CultureInfo Culture => CultureInfo.GetCultureInfo(ChosenLanguage.Filename.StartsWith("cn") ? "zh-CN" : ChosenLanguage.Filename.Split('.')[0]);
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
            bool notify = ChosenLanguage != null;
            index = ((index % _languages.Count) + _languages.Count) % _languages.Count;
            ChosenLanguage = _languages[index];

            var loadedLocale = ResourceHandler.LoadResource(ChosenLanguage.Filename);
            // Downloaded language packs can predate UI shipped with this client.
            var bundled = new LocalizationResourceHandler("Locales").LoadResource(ChosenLanguage.Filename);
            Texts = new Dictionary<string, string>(bundled?.MenuLocales ?? loadedLocale.MenuLocales);
            CardTexts = new Dictionary<string, CardLocale>(bundled?.CardLocales ?? loadedLocale.CardLocales);
            foreach (var entry in loadedLocale.MenuLocales) Texts[entry.Key] = entry.Value;
            foreach (var entry in loadedLocale.CardLocales) CardTexts[entry.Key] = entry.Value;
            if (notify) LanguageChanged?.Invoke();

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
