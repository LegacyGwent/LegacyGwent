using System;
using System.Collections.Generic;
using Autofac;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.Localization.SceneTranslating
{
    public class GameObjectTranslator : MonoBehaviour
    {
        private ITranslator _translationManager;
        public List<Translatable> ObjectsToTranslate;
        private void Start()
        {
            _translationManager = DependencyResolver.Container.Resolve<ITranslator>();
            TranslateAll();
        }
        public void TranslateAll()
        {
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
