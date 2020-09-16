using Assets.Script.Localization.Serializables;
using System.Collections.Generic;
using Assets.Script.ResourceManagement;
using Assets.Script.ResourceManagement.Serializables;
using UnityEngine;

namespace Assets.Script.Localization
{
    class AudioLocalization
    {
        private IList<ConfigEntry> _languages;
        public ConfigEntry ChosenLanguage { get; private set; }
        private ResourceHandler _resourceHandler = new ResourceHandler("Voicelines");

        public AudioLocalization()
        {
            _languages = _resourceHandler.LoadResource<IList<ConfigEntry>>(ChosenLanguage.Filename);
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
