using System;
using System.Collections.Generic;
using Assets.Script.ResourceManagement.Serializables;
using JetBrains.Annotations;

namespace Assets.Script.Localization.Serializables
{
    [Serializable]
    public class GameLocale
    {
        public ConfigEntry Info; 
        public IDictionary<string, string> MenuLocales;
        public IDictionary<string, CardLocale> CardLocales;
    }
}
