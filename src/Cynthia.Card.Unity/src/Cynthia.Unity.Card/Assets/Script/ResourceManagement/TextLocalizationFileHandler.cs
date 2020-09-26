using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using Cynthia.Card.Common.Models.Localization;

namespace Assets.Script.ResourceManagement
{
    public class TextLocalizationFileHandler : ILocalizationResourceHandler
    {
        private string _directoryPath;
        public TextLocalizationFileHandler(string dirPath)
        {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
            _directoryPath = $"{Application.dataPath}/StreamingFile/{dirPath}";
#elif UNITY_ANDROID
            _directoryPath = Application.persistentDataPath;
#endif
        }
        public IList<ConfigEntry> LoadConfiguration()
        {
            List<ConfigEntry> output = null;
            var filePath = $"{_directoryPath}/config.json";
            if (File.Exists(filePath))
            {
                var infoSerialized = File.ReadAllText(filePath);
                output = JsonConvert.DeserializeObject<List<ConfigEntry>>(infoSerialized);
            }
            return output;
        }
        public GameLocale LoadResource(string filename)
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
            var config = new List<ConfigEntry>();

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

        public bool AreFilesDownloaded()
        {
            var config = LoadConfiguration();
            return config != null && config.All(c => File.Exists($"{_directoryPath}/{c.Filename}.json"));
        }
    }
}
