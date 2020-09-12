﻿using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using UnityEngine;

namespace Assets.Script.SettingMenu
{
    class LanguageValuesInit : MonoBehaviour
    {
        private ITranslator _languageManager;

        private void Start()
        {
            _languageManager = DependencyResolver.Container.Resolve<ITranslator>();
            _languageManager.OnInit += InitLanguageOptions;
            InitLanguageOptions();
        }
        public void InitLanguageOptions()
        {
            var values = GetComponent<ChoseValue>();
            values.ChoseList = _languageManager.LanguageNames;
            values.Index = _languageManager.GameLanguage;
        }
    }
}
