using Newtonsoft.Json;
using System.Collections.Generic;
using Assets.Script.Localization.Serializables;
using Assets.Script.ResourceManagement.Serializables;
using UnityEngine;

namespace Assets.Script.ResourceManagement
{
    class LocalizationResourceHandler : ILocalizationResourceHandler
    {
        private string _directoryPath;
        public LocalizationResourceHandler(string dirPath)
        {
            _directoryPath = dirPath;
        }

        public IList<ConfigEntry> LoadConfiguration()
        {
            List<ConfigEntry> output = null;
            var filePath = $"{_directoryPath}/config";
            var file = Resources.Load<TextAsset>(filePath);

            if (file != null)
            {
                var infoSerialized = file.text;
                output = JsonConvert.DeserializeObject<List<ConfigEntry>>(infoSerialized);
            }
            return output;
        }

        public GameLocale LoadResource(string filename)
        {
            GameLocale output = null;
            var filePath = $"{_directoryPath}/{filename}";
            var file = Resources.Load<TextAsset>(filePath);

            if (file != null)
            {
                var infoSerialized = file.text;
                output = JsonConvert.DeserializeObject<GameLocale>(infoSerialized);
            }
            return output;
        }
    }
}
