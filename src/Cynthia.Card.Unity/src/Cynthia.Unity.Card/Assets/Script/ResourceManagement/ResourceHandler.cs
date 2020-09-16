using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;

namespace Assets.Script.ResourceManagement
{
    class ResourceHandler
    {
        private string _directoryPath;
        public ResourceHandler(string dirPath)
        {
            _directoryPath = dirPath;
        }

        public IList<T> LoadConfiguration<T>()
        {
            List<T> output = null;
            var filePath = $"{_directoryPath}/config";
            var file = Resources.Load<TextAsset>(filePath);

            if (file != null)
            {
                var infoSerialized = file.text;
                output = JsonConvert.DeserializeObject<List<T>>(infoSerialized);
            }
            return output;
        }

        public T LoadResource<T>(string filename)
        {
            T output = default(T);
            var filePath = $"{_directoryPath}/{filename}";
            var file = Resources.Load<TextAsset>(filePath);

            if (file != null)
            {
                var infoSerialized = file.text;
                output = JsonConvert.DeserializeObject<T>(infoSerialized);
            }
            return output;
        }
    }
}
