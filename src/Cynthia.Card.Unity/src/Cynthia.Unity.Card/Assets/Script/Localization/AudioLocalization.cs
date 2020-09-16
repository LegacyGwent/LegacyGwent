using Assets.Script.ResourceManagement;
using Assets.Script.ResourceManagement.Serializables;
using System.Collections.Generic;
using UnityEngine;

namespace Assets.Script.Localization
{
    class AudioLocalization
    {
        private IList<ConfigEntry> _languages;
        public ConfigEntry ChosenLanguage { get; private set; }
        public int ChosenLanguageIndex => _languages.IndexOf(ChosenLanguage);
        public LocalizationResourceHandler ResourceHandler = new LocalizationResourceHandler("Voicelines");

        public AudioLocalization()
        {
            _languages = ResourceHandler.LoadConfiguration();
            ChooseLanguage(PlayerPrefs.GetInt("AudioLanguage", 0));
        }
        public int ChooseLanguage(int index)
        {
            index %= _languages.Count;
            ChosenLanguage = _languages[index];
            return index;
        }
    }
}
