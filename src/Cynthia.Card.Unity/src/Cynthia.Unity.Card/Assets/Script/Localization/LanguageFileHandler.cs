using Assets.Script.Localization.Serializables;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Assets.Script.Localization
{
    public class LanguageFileHandler
    {
        public List<LocaleInfo> LoadLanguagesConfig()
        {
            List<LocaleInfo> output = null;
            var infoSerialized = Resources.Load<TextAsset>("Locales/config").text;
            if (infoSerialized != null)
            {
                output = JsonConvert.DeserializeObject<List<LocaleInfo>>(infoSerialized);
            }
            return output;
        }

        public GameLocale LoadLocale(string filename)
        {
            GameLocale output = null;
            var localeFile = Resources.Load<TextAsset>($"Locales/{filename}");
            if (localeFile != null)
            {
                output = JsonConvert.DeserializeObject<GameLocale>(localeFile.text);
            }
            return output;
        }

        public void SaveGameLocales(IList<GameLocale> locales)
        {
            var config = new List<LocaleInfo>();
            foreach (var locale in locales)
            {
                config.Add(locale.Info);

                var serializedLocale = JsonConvert.SerializeObject(locale);
                File.WriteAllText($"{Application.dataPath}/Locales/{locale.Info.Filename}.json", serializedLocale);
            }

            var serializedConfig = JsonConvert.SerializeObject(config);
            File.WriteAllText($"{Application.dataPath}/Locales/config.json", serializedConfig);
        }

        public bool AreFilesCorrupted()
        {
            var config = LoadLanguagesConfig();
            if (config == null)
            {
                return true;
            }

            foreach (var c in config)
            {
                if (!File.Exists($"{Application.dataPath}/Locales/{c.Filename}"))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
