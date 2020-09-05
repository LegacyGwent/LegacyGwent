using Cynthia.Card.Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using Newtonsoft.Json;
using UnityEngine;

namespace Assets.Script.Localization
{
    class LanguageManager : ITranslator
    {
        private Dictionary<string, string> Texts = new Dictionary<string, string>();
        private List<GameLocale> _locales;
        private GameLocale _gameLanguage;

        public string GetCardFlavor(string cardId)
        {
            throw new NotImplementedException();
        }

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
                SaveCardsToJson();
            }
        }

        public List<string> LanguageNames
        {
            get { return new List<string>(){"Chinese", "Japanese", "English", "Russian" };
        }
        }
        public List<string> LanguageFilenames
        {
            get
            {
                return new List<string>() {"CN", "JP", "EN", "RU"};
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

        public string GetCardName(string cardId)
        {
            throw new NotImplementedException();
        }

        public string GetCardInfo(string cardId)
        {
            throw new NotImplementedException();
        }

        public void SaveCardsToJson()
        {
            var allCardTexts = new Dictionary<string, CardTexts>();
            foreach (var key in Texts.Keys)
            {
                var value = Texts[key];
                var compounds = key.Split('_');
                if (!key.Contains("card") || key.Contains("tag"))
                {
                    continue;
                }
                if (compounds.Length != 3)
                {
                    continue;
                }
                try
                {
                    int a = int.Parse(compounds[1]);
                }
                catch (Exception e)
                {
                    continue;
                }

                var id = compounds[1];
                if (!allCardTexts.ContainsKey(id))
                {
                    allCardTexts.Add(id, new CardTexts());
                }
                switch (compounds[2])
                {
                    case "name":
                        allCardTexts[id].Name = value; 
                        break;
                    case "info":
                        allCardTexts[id].Info = value;
                        break;
                    case "flavor":
                        allCardTexts[id].Flavor = value;
                        break;
                }
                
            }

            var output = JsonConvert.SerializeObject(allCardTexts);
            using (var writer = new StreamWriter($"{GameLanguage}.json"))
            {
                writer.Write(output);
            }
        }
    }
}
