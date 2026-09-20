using Autofac;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Localization
{
    // Retain keys and arguments so existing labels update when the language changes.
    public sealed class LocalizedLabel : MonoBehaviour
    {
        private Text label;
        private string key;
        private object[] arguments;

        public static string Get(string id, params object[] args)
        {
            var locale = DependencyResolver.Container.Resolve<LocalizationService>().TextLocalization;
            return args.Length == 0 ? locale.GetText(id) : string.Format(locale.Culture, locale.GetText(id), args);
        }

        public static void Set(Text target, string id, params object[] args)
        {
            var binding = target.GetComponent<LocalizedLabel>();
            if (binding == null) binding = target.gameObject.AddComponent<LocalizedLabel>();
            binding.label = target;
            binding.key = id;
            binding.arguments = args;
            binding.Refresh();
        }

        private void OnEnable() { TextLocalization.LanguageChanged += Refresh; Refresh(); }
        private void OnDisable() { TextLocalization.LanguageChanged -= Refresh; }
        private void Refresh()
        {
            if (label != null && key != null) label.text = Get(key, arguments);
        }
    }
}
