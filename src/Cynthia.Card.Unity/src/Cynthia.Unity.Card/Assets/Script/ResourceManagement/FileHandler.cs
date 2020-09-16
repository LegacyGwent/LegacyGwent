using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Assets.Script.Localization
{
    public class FileHandler
    {
        protected string _directoryPath;
        public FileHandler(string dirPath)
        {
            _directoryPath = dirPath;
        }
        public List<T> LoadConfig<T>()
        {
            List<T> output = null;
            var filePath = $"{_directoryPath}/config.json";
            if (File.Exists(filePath))
            {
                var infoSerialized = File.ReadAllText(filePath);
                output = JsonConvert.DeserializeObject<List<T>>(infoSerialized);
            }
            return output;
        }
    }
}
