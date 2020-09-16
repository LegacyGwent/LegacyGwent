using Assets.Script.Localization.Serializables;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace Assets.Script.Localization
{
    public class LocalizationFileHandler
    {
        private readonly string _directoryPath = $"{Application.dataPath}/StreamingFile/Locales";
        public List<LocaleInfo> LoadLanguagesConfig()
        {
            List<LocaleInfo> output = null;

            var filePath = $"{_directoryPath}/config.json";
            if (File.Exists(filePath))
            {
                var infoSerialized = File.ReadAllText(filePath);
                output = JsonConvert.DeserializeObject<List<LocaleInfo>>(infoSerialized);
            }

            return output;
        }

        public GameLocale LoadLocale(string filename)
        {
            GameLocale output = null;

            var filePath = $"{_directoryPath}/{filename}.json";
            if (File.Exists(filePath))
            {
                var localeSerialized = File.ReadAllText(filePath);
                output = JsonConvert.DeserializeObject<GameLocale>(localeSerialized);
            }

            return output;
        }

        public void SaveGameLocales(IList<GameLocale> locales)
        {
            var config = new List<LocaleInfo>();

            if (!Directory.Exists(_directoryPath))
            {
                Directory.CreateDirectory(_directoryPath);
            }

            foreach (var locale in locales)
            {
                config.Add(locale.Info);
                var serializedLocale = JsonConvert.SerializeObject(locale);
                File.WriteAllText($"{_directoryPath}/{locale.Info.Filename}.json", serializedLocale);
            }

            var serializedConfig = JsonConvert.SerializeObject(config);
            File.WriteAllText($"{_directoryPath}/config.json", serializedConfig);
        }

        public bool AreFilesCorrupted()
        {
            var config = LoadLanguagesConfig();
            return config == null || config.Any(c => !File.Exists($"{_directoryPath}/{c.Filename}"));
        }
    }
}
