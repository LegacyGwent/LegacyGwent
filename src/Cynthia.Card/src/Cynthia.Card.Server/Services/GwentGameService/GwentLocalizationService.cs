using System.Collections.Generic;
using System.IO;
using Cynthia.Card.Common.Models.Localization;
using Newtonsoft.Json;

namespace Cynthia.Card.Server.Services.GwentGameService
{
    public class GwentLocalizationService
    {
        private string _gameLocales;
        public GwentLocalizationService()
        {
            var config = new List<ConfigEntry>();
            var currentDirectory = Directory.GetCurrentDirectory();
            using (var stream = new StreamReader($"{currentDirectory}/Locales/config.json"))
            {
                var serializedConfig = stream.ReadToEnd();
                config = JsonConvert.DeserializeObject<List<ConfigEntry>>(serializedConfig);
            }

            var loadedLocales = new List<GameLocale>();
            foreach (var locale in config)
            {
                var filePath = $"{currentDirectory}/Locales/{locale.Filename}.json";
                var loadedLocale = JsonConvert.DeserializeObject<GameLocale>(File.ReadAllText(filePath));
                loadedLocales.Add(loadedLocale);
            }

            _gameLocales = JsonConvert.SerializeObject(loadedLocales);
        }
        public string GetGameLocales()
        {
            return _gameLocales;
        }
    }
}
