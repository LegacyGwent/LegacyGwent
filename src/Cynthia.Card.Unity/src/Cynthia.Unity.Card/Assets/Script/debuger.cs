using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEditor.Experimental.AssetImporters;

public class Debuger : EditorWindow
{
    [MenuItem("Tools/Debuger")]
    public static void ShowWindow()
    {
        GetWindow(typeof(Debuger), false, "Find Missing");
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Find Missing Fonts in Current Scene"))
        {
            FindMissingFontsInScene();
        }

        if (GUILayout.Button("Find Missing Fonts in Prefabs"))
        {
            FindMissingFontsInPrefabs();
        }

        if (GUILayout.Button("Find Missing Prefab References in Current Scene"))
        {
            FindMissingPrefabReferencesInScene();
        }

        if (GUILayout.Button("Find Missing Prefab References in Prefabs"))
        {
            FindMissingPrefabReferencesInPrefabs();
        }
    }

    private static void FindMissingFontsInScene()
    {
        Debug.Log("Checking scene for missing fonts...");
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>(); // Includes inactive GameObjects
        int missingFontCount = CheckGameObjectsForMissingFonts(allGameObjects);

        Debug.Log($"Scene font check complete! Found {missingFontCount} text components with missing fonts.");
    }

    private static void FindMissingFontsInPrefabs()
    {
        Debug.Log("Checking prefabs for missing fonts...");
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        int missingFontCount = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                missingFontCount += CheckGameObjectsForMissingFonts(new GameObject[] { prefab });
            }
        }

        Debug.Log($"Prefab font check complete! Found {missingFontCount} text components with missing fonts.");
    }

    private static void FindMissingPrefabReferencesInScene()
    {
        Debug.Log("Checking scene for missing prefab references...");
        GameObject[] allGameObjects = Resources.FindObjectsOfTypeAll<GameObject>(); // Includes inactive GameObjects
        int missingPrefabCount = CheckGameObjectsForMissingPrefabs(allGameObjects);

        Debug.Log($"Scene prefab check complete! Found {missingPrefabCount} game objects with missing prefabs.");
    }

    private static void FindMissingPrefabReferencesInPrefabs()
    {
        Debug.Log("Checking prefabs for missing prefab references...");
        string[] prefabGUIDs = AssetDatabase.FindAssets("t:Prefab");
        int missingPrefabCount = 0;

        foreach (string guid in prefabGUIDs)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);

            if (prefab != null)
            {
                missingPrefabCount += CheckGameObjectsForMissingPrefabs(new GameObject[] { prefab });
            }
        }

        Debug.Log($"Prefab prefab check complete! Found {missingPrefabCount} prefabs with missing references.");
    }

    private static int CheckGameObjectsForMissingFonts(GameObject[] gameObjects)
    {
        int missingFontCount = 0;

        foreach (GameObject go in gameObjects)
        {
            if (go == null) continue;

            // Build full hierarchy path and identify the root object
            string hierarchyPath = GetFullHierarchyPath(go);
            string rootObject = GetRootObject(go).name;

            // Check Unity UI Text
            Text uiText = go.GetComponent<Text>();
            if (uiText != null && uiText.font == null)
            {
                Debug.LogWarning(
                    $"[MISSING FONT] Type: Unity UI Text | GameObject: {hierarchyPath} | Root Object: {rootObject} | Active: {go.activeInHierarchy}",
                    go
                );
                missingFontCount++;
            }

            // Check TextMeshProUGUI
            TextMeshProUGUI textMeshProUGUI = go.GetComponent<TextMeshProUGUI>();
            if (textMeshProUGUI != null && textMeshProUGUI.font == null)
            {
                Debug.LogWarning(
                    $"[MISSING FONT] Type: TextMeshProUGUI | GameObject: {hierarchyPath} | Root Object: {rootObject} | Active: {go.activeInHierarchy}",
                    go
                );
                missingFontCount++;
            }

            // Check TextMesh
            TextMesh textMesh = go.GetComponent<TextMesh>();
            if (textMesh != null && textMesh.font == null)
            {
                Debug.LogWarning(
                    $"[MISSING FONT] Type: TextMesh | GameObject: {hierarchyPath} | Root Object: {rootObject} | Active: {go.activeInHierarchy}",
                    go
                );
                missingFontCount++;
            }
        }

        return missingFontCount;
    }

    private static int CheckGameObjectsForMissingPrefabs(GameObject[] gameObjects)
    {
        int missingPrefabCount = 0;

        foreach (GameObject go in gameObjects)
        {
            if (go == null) continue;

            // Check if GameObject is a prefab instance and if the prefab is missing
            var prefabAssetType = PrefabUtility.GetPrefabAssetType(go);
            if (prefabAssetType == PrefabAssetType.NotAPrefab)
                continue; // Skip non-prefab objects

            GameObject prefabInstance = PrefabUtility.GetPrefabParent(go) as GameObject;
            if (prefabInstance == null)
            {
                Debug.LogWarning(
                    $"[MISSING PREFAB] GameObject: {go.name} | Hierarchy Path: {GetFullHierarchyPath(go)} | Root Object: {GetRootObject(go).name} | Active: {go.activeInHierarchy}",
                    go
                );
                missingPrefabCount++;
            }
        }

        return missingPrefabCount;
    }

    private static string GetFullHierarchyPath(GameObject go)
    {
        string path = go.name;
        Transform current = go.transform;

        while (current.parent != null)
        {
            current = current.parent;
            path = $"{current.name}/{path}";
        }

        return path;
    }

    private static GameObject GetRootObject(GameObject go)
    {
        Transform current = go.transform;

        // Find the root of the hierarchy
        while (current.parent != null)
        {
            current = current.parent;
        }

        return current.gameObject;
    }
}
