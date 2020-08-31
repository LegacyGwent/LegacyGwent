using Cynthia.Card.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;

namespace Assets.Script.Localization
{
    class LanguageManager : ITranslator
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

        public LanguageManager()
        {
            _locales = LoadLocales();
            GameLanguage = PlayerPrefs.GetInt("language", 0);
        }

        private List<GameLocale> LoadLocales()
        {
            var output = new List<GameLocale>();
            var localeInfo = Resources.Load<TextAsset>("Locales/info");
            var document = new XmlDocument();
            document.Load(new StringReader(localeInfo.text));
            if (document.DocumentElement == null)
                return output;

            var nodes = document.DocumentElement.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                output.Add(new GameLocale { Filename = node.InnerText });
            }

            return output;
        }

        private void LoadTexts()
        {
            Texts.Clear();

            var localeFile = Resources.Load<TextAsset>($"Locales/{_gameLanguage.Filename}");
            var document = new XmlDocument();
            document.Load(new StringReader(localeFile.text));
            if (document.DocumentElement == null)
                return;

            var nodes = document.DocumentElement.ChildNodes;
            foreach (XmlNode node in nodes)
            {
                if (node.Attributes == null)
                    continue;

                var id = node.Attributes["id"].Value;
                var val = node.InnerText;
                if (Texts.ContainsKey(id))
                {
                    throw new ArgumentException(
                        $"An item with the same key has already been added. Locale file has two entries with the same id : \"{id}\"");
                }
                if (!string.IsNullOrWhiteSpace(id))
                {
                    Texts.Add(id, val);
                }
            }
        }
        public string GetText(string id)
        {
            return Texts.ContainsKey(id) ? Texts[id] : id;
        }
    }
}
