using Assets.Script.Localization.Serializables;
using Assets.Script.ResourceManagement;
using System.Collections.Generic;
using Assets.Script.ResourceManagement.Serializables;
using UnityEngine;

namespace Assets.Script.Localization
{
    class TextLocalization
    {
        private IList<ConfigEntry> _languages;
        public ConfigEntry ChosenLanguage { get; private set; }

        private ResourceHandler _resourceHandler = new ResourceHandler("Locales");

        public IDictionary<string, string> Texts;
        public IDictionary<string, CardLocale> CardTexts;

        public TextLocalization()
        {
            _languages = _resourceHandler.LoadConfiguration<ConfigEntry>();
            ChooseLanguage(PlayerPrefs.GetInt("TextLanguage", 0));

        }
        public int ChooseLanguage(int index)
        {
            index %= _languages.Count;
            ChosenLanguage = _languages[index];

            var loadedLocale = _resourceHandler.LoadResource<GameLocale>(ChosenLanguage.Filename);
            Texts = loadedLocale.MenuLocales;
            CardTexts = loadedLocale.CardLocales;

            return index;
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
