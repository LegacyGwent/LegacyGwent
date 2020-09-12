using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace Assets.Script.Localization.Serializables
{
    [Serializable]
    public class GameLocale
    {
        public LocaleInfo Info; 
        public IDictionary<string, string> MenuLocales;
        public IDictionary<string, CardLocale> CardLocales;
    }
}
