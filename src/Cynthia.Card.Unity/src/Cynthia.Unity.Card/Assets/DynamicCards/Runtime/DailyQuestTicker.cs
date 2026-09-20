using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Script.DynamicCards
{
    public sealed class DailyQuestTicker : MonoBehaviour
    {
        private float next;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
        private static void Install()
        {
            // Isolated tool scenes do not install account services. Login bootstrap runs in Awake.
            if(DependencyResolver.Container==null)return;
            var root=new GameObject("Daily quest synchronization");
            DontDestroyOnLoad(root); root.AddComponent<DailyQuestTicker>();
            InitializeMainMenu(SceneManager.GetActiveScene());
        }
        private void OnEnable() { SceneManager.sceneLoaded+=SceneLoaded; }
        private void OnDisable() { SceneManager.sceneLoaded-=SceneLoaded; }
        private static void InitializeMainMenu(Scene scene)
        {
            if(scene.name!="Game")return;
            // EditorInfo lives on the inactive deck builder. The task entry must exist
            // before the player first opens that UI, so initialize from the loaded scene.
            foreach(var root in scene.GetRootGameObjects())
                foreach(var editor in root.GetComponentsInChildren<EditorInfo>(true))
                    DailyQuestPanel.EnsureInitialized(editor);
        }
        private void SceneLoaded(Scene scene,LoadSceneMode mode)
        {
            InitializeMainMenu(scene);
            if(scene.name=="Game" || scene.name=="GamePlay") _=DailyQuestClient.Refresh();
        }
        private void Update()
        {
            if(Time.realtimeSinceStartup<next)return;
            next=Time.realtimeSinceStartup+60;
            _=DailyQuestClient.Refresh();
        }
    }
}
