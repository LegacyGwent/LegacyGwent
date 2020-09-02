using System.Collections.Generic;
using Assets.Script.Localization;
using Autofac;
using Cynthia.Card.Common.Models;
using UnityEngine;

namespace Assets.Script.SettingMenu
{
    class LanguageValuesInit : MonoBehaviour
    {
        private void Start()
        {
            var values = GetComponent<ChoseValue>();
            var languageManager = DependencyResolver.Container.Resolve<ITranslator>();
            values.ChoseList = languageManager.LanguageNames;
            values.Index = languageManager.GameLanguage;
        }
    }
}
