global using Random = UnityEngine.Random;
using System.Collections;
namespace UnityEngine
{
    public class Object
    {
        protected static T FindObjectOfType<T>() where T : class => null;
        protected static GameObject Instantiate(GameObject source, Transform parent, bool world) => new GameObject();
        protected static void Destroy(GameObject target) { }
    }
    public class Component : Object
    {
        private GameObject _object;
        public GameObject gameObject { get => _object ??= new GameObject(); set => _object = value; }
        public Transform transform => gameObject.transform;
    }
    public class MonoBehaviour : Component
    {
        public List<IEnumerator> Coroutines = new();
        protected void StartCoroutine(IEnumerator coroutine) => Coroutines.Add(coroutine);
    }
    public class GameObject : Object
    {
        public bool activeSelf;
        public Transform transform;
        public GameObject(string name = "", params Type[] components) { transform = new RectTransform { gameObject = this }; }
        public void SetActive(bool active) => activeSelf = active;
        public T GetComponent<T>() where T : new() => new T();
    }
    public class Transform : Component
    {
        public Vector3 localScale, eulerAngles;
        public void SetParent(Transform parent, bool world) { }
        public void SetSiblingIndex(int index) { }
        public int GetSiblingIndex() => 0;
        public Transform Find(string name) => null;
        public T GetComponent<T>() where T : new() => new T();
    }
    public class RectTransform : Transform { public Vector2 sizeDelta, anchorMin, anchorMax, anchoredPosition; }
    public struct Vector2 { public float x, y; public Vector2(float x, float y) { this.x = x; this.y = y; } public static Vector2 zero => default; }
    public struct Vector3 { public Vector3(float x, float y, float z) { } }
    public static class Time { public static float unscaledDeltaTime = 1; }
    public static class Random { public static int Range(int min, int max) => min; }
}
namespace UnityEngine.UI
{
    public class Text : UnityEngine.Component { public string text; }
    public class Slider : UnityEngine.Component { public bool interactable; public float minValue, maxValue, value; }
    public static class LayoutRebuilder { public static void ForceRebuildLayoutImmediate(UnityEngine.RectTransform rect) { } }
}
namespace Assets.Script.Localization { public class LocalizationService { public string GetText(string key) => key; } }
namespace Autofac { public static class ResolveExtension { public static T Resolve<T>(this object container) where T : new() => new T(); } }
public static class DependencyResolver { public static object Container = new object(); }
public class RopeController { public UnityEngine.Component rope; }
