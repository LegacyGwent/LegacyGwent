using System;
using System.Collections.Generic;
using Autofac;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Localization.SceneTranslating
{
    public class GameObjectTranslator : MonoBehaviour
    {
        private LocalizationService _translationManager;
        public List<Translatable> ObjectsToTranslate;
        private void Start()
        {
            _translationManager = DependencyResolver.Container.Resolve<LocalizationService>();
            TranslateAll();
        }
        private void OnEnable() { TextLocalization.LanguageChanged += TranslateAll; if (_translationManager != null) TranslateAll(); }
        private void OnDisable() { TextLocalization.LanguageChanged -= TranslateAll; }
        public void TranslateAll()
        {
            if (_translationManager == null)
                _translationManager = DependencyResolver.Container.Resolve<LocalizationService>();
            foreach (var entry in ObjectsToTranslate)
            {
                var textId = entry.Id;
                try
                {
                    var textContent = entry.TextObject.GetComponent<Text>();
                    textContent.text = _translationManager.GetText(textId);
                }
                catch (NullReferenceException exception)
                {
                    Debug.Log($"{exception.Message}. Faulty object is: {entry.TextObject.name}");
                }
            }
        }
    }
}
