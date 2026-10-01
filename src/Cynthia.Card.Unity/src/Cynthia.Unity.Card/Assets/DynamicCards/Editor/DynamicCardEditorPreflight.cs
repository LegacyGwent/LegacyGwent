using System;
using UnityEditor;
using UnityEngine;

namespace Assets.Script.DynamicCards.Editor
{
    [InitializeOnLoad]
    public sealed class DynamicCardEditorPreflight : EditorWindow
    {
        private DynamicCardEditorContentPolicy.Status status;
        private Vector2 scroll;

        static DynamicCardEditorPreflight()
        {
            EditorApplication.delayCall += StartupCheck;
            EditorApplication.playModeStateChanged += state =>
            {
                if (state == PlayModeStateChange.ExitingEditMode) Diagnose("Play Mode");
            };
        }

        private static void StartupCheck()
        {
            if (!Application.isBatchMode && !EditorApplication.isCompiling && !BuildPipeline.isBuildingPlayer)
                Diagnose("Editor startup");
        }

        private static void Diagnose(string context)
        {
            // Read the catalog and file presence once; do not instantiate card sources or
            // build packages, force user quality, or block an unrelated lab scene.
            if (!DynamicCardSettings.Enabled) return;
            var result = DynamicCardEditorContentPolicy.Evaluate();
            if (result.Source != "bundles")
                Debug.LogWarning("Dynamic card " + context + ": " + result.Message +
                    " [" + result.Reason + "]. Inspect Tools > Dynamic Cards > Preview Status.");
        }

        [MenuItem("Tools/Dynamic Cards/Preview Status")]
        public static void Open()
        {
            GetWindow<DynamicCardEditorPreflight>("Premium preview").RefreshStatus();
        }

        private void OnEnable() { RefreshStatus(); }
        private void RefreshStatus()
        {
            status = DynamicCardEditorContentPolicy.Evaluate();
            Repaint();
        }

        private void OnGUI()
        {
            scroll = EditorGUILayout.BeginScrollView(scroll);
            EditorGUILayout.LabelField("闪卡开发预览 / Premium editor preview", EditorStyles.boldLabel);
            var current = DynamicCardEditorContentPolicy.Mode;
            var selected = (DynamicCardEditorContentMode)EditorGUILayout.EnumPopup("预览来源", current);
            if (selected != current)
            {
                DynamicCardEditorContentPolicy.Mode = selected;
                RefreshStatus();
            }
            EditorGUILayout.HelpBox("Automatic：完整缓存优先，缓存不可用时读取工程源资源。BundlesOnly：仅缓存，用于性能与资源包验收。SourceOnly：强制工程源预览。修改后重新进入 Play Mode。", MessageType.Info);
            if (status == null) RefreshStatus();
            EditorGUILayout.LabelField("当前品质", DynamicCardSettings.Quality.ToString());
            if (!DynamicCardSettings.Enabled)
                EditorGUILayout.HelpBox("闪卡品质为 Off；预览来源设置不会开启闪卡。需要动画时请在游戏设置中手动选择品质。", MessageType.Info);
            EditorGUILayout.LabelField("实际来源", status.Source);
            EditorGUILayout.LabelField("状态原因", status.Reason);
            EditorGUILayout.LabelField("目录卡数量", status.CatalogEntries.ToString());
            EditorGUILayout.LabelField("缺失源预制体", status.MissingSourcePrefabs.ToString());
            EditorGUILayout.HelpBox(status.Message, status.Source == "unavailable" ? MessageType.Error :
                status.Source == "source" ? MessageType.Warning : MessageType.Info);
            if (status.Source == "source")
                EditorGUILayout.HelpBox("当前是源资源预览验收；同步读取可能使收藏页面停顿。它不证明缓存资源包或 Player 已通过验收。", MessageType.Warning);
            EditorGUILayout.LabelField("缓存目录", status.BundleRoot);
            EditorGUILayout.HelpBox("若源资源缺失，在干净 checkout 的仓库根目录运行 python scripts/premium-content.py restore。不要用该命令覆盖已有自制源资源。若需缓存验收，打开 Build Options 手动构建目标平台资源包；不会自动重建。", MessageType.Info);
            if (GUILayout.Button("重新检查")) RefreshStatus();
            if (GUILayout.Button("打开 Build Options")) DynamicCardBuildWindow.Open();
            EditorGUILayout.EndScrollView();
        }
    }
}
