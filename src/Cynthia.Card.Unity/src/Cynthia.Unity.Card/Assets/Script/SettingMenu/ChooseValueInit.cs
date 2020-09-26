using Assets.Script.Localization;
using Autofac;
using System.Linq;
using UnityEngine;

namespace Assets.Script.SettingMenu
{
    class ChooseValueInit : MonoBehaviour
    {
        public void LoadValues()
        {
            var values = GetComponent<ChoseValue>();
            var localizationService = DependencyResolver.Container.Resolve<LocalizationService>();
            var resourceLoader = localizationService.TextLocalization.ResourceHandler;
            values.ChoseList = resourceLoader.LoadConfiguration().Select(c => c.Name).ToList();
        }
    }
}
