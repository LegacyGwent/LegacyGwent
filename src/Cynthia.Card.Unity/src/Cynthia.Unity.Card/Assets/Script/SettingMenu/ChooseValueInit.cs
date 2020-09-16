using Assets.Script.ResourceManagement;
using System.Linq;
using UnityEngine;

namespace Assets.Script.SettingMenu
{
    class ChooseValueInit : MonoBehaviour
    {
        public string ValuesPath;

        public void LoadValues()
        {
            var values = GetComponent<ChoseValue>();
            var resourceLoader = new LocalizationResourceHandler(ValuesPath);
            values.ChoseList = resourceLoader.LoadConfiguration().Select(c => c.Name).ToList();
        }
    }
}
