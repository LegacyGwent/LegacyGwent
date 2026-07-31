using Assets.Script.ResourceManagement;
using System;
using System.Collections.Generic;
using Cynthia.Card.Common.Models.Localization;
using UnityEngine;

namespace Assets.Script.Localization
{
    class AudioLocalization
    {
        public const string PreferenceKey = "DiyAi.AudioLanguage";

        private IList<ConfigEntry> _languages;
        public ConfigEntry ChosenLanguage { get; private set; }
        public int ChosenLanguageIndex => _languages.IndexOf(ChosenLanguage);
        public LocalizationResourceHandler ResourceHandler = new LocalizationResourceHandler("Voicelines");

        public AudioLocalization()
        {
            _languages = ResourceHandler.LoadConfiguration();
            ChooseLanguage(PlayerPrefs.GetInt(PreferenceKey, GetChineseLanguageIndex()));
        }
        private int GetChineseLanguageIndex()
        {
            for (var index = 0; index < _languages.Count; index++)
            {
                if (string.Equals(_languages[index].Filename, "cn", StringComparison.OrdinalIgnoreCase))
                    return index;
            }
            return 0;
        }
        public int ChooseLanguage(int index)
        {
            index %= _languages.Count;
            ChosenLanguage = _languages[index];
            return index;
        }
    }
}
