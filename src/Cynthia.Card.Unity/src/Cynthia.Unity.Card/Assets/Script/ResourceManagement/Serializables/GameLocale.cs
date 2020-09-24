using System;
using System.Collections.Generic;

namespace Assets.Script.ResourceManagement.Serializables
{
    [Serializable]
    public class GameLocale
    {
        public ConfigEntry Info;
        public IDictionary<string, string> MenuLocales;
        public IDictionary<string, CardLocale> CardLocales;
    }
}
