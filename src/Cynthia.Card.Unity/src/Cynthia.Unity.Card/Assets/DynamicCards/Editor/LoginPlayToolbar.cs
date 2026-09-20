using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UIElements;

namespace Assets.Script.DynamicCards.Editor
{
    [InitializeOnLoad]
    public static class LoginPlayToolbar
    {
        private const string LoginScene = "Assets/Resources/Scenes/LoginScene.unity";
        private static ScriptableObject toolbar;
        static LoginPlayToolbar()
        {
            EditorApplication.update += Install;
            EditorApplication.playModeStateChanged += state =>
            {
                if ((state == PlayModeStateChange.EnteredPlayMode || state == PlayModeStateChange.EnteredEditMode) && SessionState.GetBool("LegacyLoginStart", false))
                {
                    string previous = SessionState.GetString("LegacyLoginPrevious", "");
                    EditorSceneManager.playModeStartScene = string.IsNullOrEmpty(previous) ? null : AssetDatabase.LoadAssetAtPath<SceneAsset>(previous);
                    SessionState.SetBool("LegacyLoginStart", false);
                }
            };
        }

        private static void Install()
        {
            if (toolbar != null) return;
            var type = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.Toolbar");
            if (type == null) return;
            foreach (var candidate in Resources.FindObjectsOfTypeAll(type))
            {
                var viewType = typeof(UnityEditor.Editor).Assembly.GetType("UnityEditor.GUIView");
                var property = viewType == null ? null : viewType.GetProperty("visualTree", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                var root = property == null ? null : property.GetValue(candidate, null) as VisualElement;
                if (root == null) continue;
                var previous = root.Q<VisualElement>("LegacyGwentLoginPlay");
                if (previous != null) previous.RemoveFromHierarchy();
                var button = new IMGUIContainer(Draw) { name = "LegacyGwentLoginPlay" };
                button.style.position = Position.Absolute;
                button.style.left = Length.Percent(50); button.style.marginLeft = -190;
                button.style.top = 3; button.style.width = 112; button.style.height = 24;
                root.Add(button); toolbar = candidate as ScriptableObject;
                Debug.Log("[LegacyGwent] Login toolbar shortcut ready."); break;
            }
        }

        private static void Draw()
        {
            using (new EditorGUI.DisabledScope(EditorApplication.isPlayingOrWillChangePlaymode || EditorApplication.isCompiling))
                if (GUILayout.Button(new GUIContent("▶ Login", "从 LoginScene 启动游戏"), EditorStyles.toolbarButton, GUILayout.Height(22))) PlayLogin();
        }

        [MenuItem("Tools/Legacy Gwent/Play from Login %#l")]
        public static void PlayLogin()
        {
            if (EditorApplication.isPlayingOrWillChangePlaymode) return;
            // Unity preserves the currently open scene setup when using a play-mode start scene.
            SessionState.SetString("LegacyLoginPrevious", AssetDatabase.GetAssetPath(EditorSceneManager.playModeStartScene));
            SessionState.SetBool("LegacyLoginStart", true);
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(LoginScene);
            if (EditorSceneManager.playModeStartScene == null) throw new InvalidOperationException("LoginScene is missing.");
            EditorApplication.isPlaying = true;
        }
    }
}
