using Cynthia.Card.Common.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Localization
{
    class LanguageManagerJson : ITranslator
    {
        private Dictionary<string, string> Texts = new Dictionary<string, string>();
        private Dictionary<string, CardTexts> CardTexts = new Dictionary<string, CardTexts>();

        private List<GameLocale> _locales;
        private GameLocale _gameLanguage;

        public int GameLanguage
        {
            get => _locales.IndexOf(_gameLanguage);
            set
            {
                if (value >= _locales.Count || value < 0)
                {
                    value = 0;
                }
                _gameLanguage = _locales[value];
                LoadTexts();
                LoadCards();
            }
        }

        public List<string> LanguageNames => _locales.Select(l => l.Name).ToList();
        public List<string> LanguageFilenames => _locales.Select(l => l.Filename).ToList();


        public LanguageManagerJson()
        {
            var infoSerialized = Resources.Load<TextAsset>("Locales/config").text;
            _locales = JsonConvert.DeserializeObject<List<GameLocale>>(infoSerialized);
            GameLanguage = PlayerPrefs.GetInt("Language", 0);
        }

        private void LoadTexts()
        {
            Texts.Clear();

            var localeFile = Resources.Load<TextAsset>($"Locales/{_gameLanguage.Filename}");
            if (localeFile != null)
            {
                Texts = JsonConvert.DeserializeObject<Dictionary<string, string>>(localeFile.text);
            }
        }

        public void LoadCards()
        {
            var languageFile = Resources.Load<TextAsset>($"Locales/{_gameLanguage.Filename}-cards");
            if (languageFile == null)
            {
                return;
            }

            var loadedCardTexts = JsonConvert.DeserializeObject<Dictionary<string, CardTexts>>(languageFile.text);
            CardTexts = loadedCardTexts;
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