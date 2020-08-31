using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cynthia.Card.Common.Models;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Script.Localization
{
    class LanguageManagerJson : ITranslator
    {
        private Dictionary<string, string> Texts = new Dictionary<string, string>();
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
            }
        }

        public LanguageManagerJson()
        {
            LoadAvailableLocales();
            GameLanguage = PlayerPrefs.GetInt("language", 0);
        }

        public void LoadAvailableLocales()
        {
            var infoSerialized = Resources.Load<TextAsset>("Locales/info").text;
            _locales = JsonConvert.DeserializeObject<List<GameLocale>>(infoSerialized);
        }

        private void LoadTexts()
        {
            Texts.Clear();

            var localeFile = Resources.Load<TextAsset>($"Locales/{_gameLanguage.Filename}");
            Texts = JsonConvert.DeserializeObject<Dictionary<string, string>>(localeFile.text);
        }

        public string GetText(string id)
        {
            return Texts.ContainsKey(id) ? Texts[id] : id;
        }
    }
}