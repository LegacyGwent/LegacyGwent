using Assets.Script.ResourceManagement;
using Assets.Script.ResourceManagement.Serializables;
using System.Linq;
using UnityEngine;

namespace Assets.Script.SettingMenu
{
    class ChooseValueInit : MonoBehaviour
    {
        public string ValuesPath;

        private void Start()
        {
            var values = GetComponent<ChoseValue>();
            var resourceLoader = new ResourceHandler(ValuesPath);
            values.ChoseList = resourceLoader.LoadConfiguration<ConfigEntry>().Select(c => c.Name).ToList();
        }
    }
}
