using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Autofac;
using Assets.Script.DynamicCards;
using Cynthia.Card.Client;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DailyQuestVerification
{
    private static string Work=>Path.GetFullPath(Path.Combine(Application.dataPath,"../../../../../work/DailyQuests"));
    private static bool running;
    static DailyQuestVerification(){EditorApplication.update+=Poll;}
    private static async void Poll()
    {
        string file=Path.Combine(Work,"ui.request");
        if(running || EditorApplication.isCompiling || !File.Exists(file))return;
        string mode;try{mode=File.ReadAllText(file).Trim();File.Delete(file);}catch(IOException){return;}
        running=true;
        try
        {
            foreach(var guid in AssetDatabase.FindAssets("t:Texture2D",new[]{"Assets/Resources/DailyQuests"}))
            {
                var importer=(TextureImporter)AssetImporter.GetAtPath(AssetDatabase.GUIDToAssetPath(guid));
                if(importer.textureType!=TextureImporterType.Sprite){importer.textureType=TextureImporterType.Sprite;importer.spriteImportMode=SpriteImportMode.Single;importer.SaveAndReimport();}
            }
            if(mode=="prepare")return;
            if(!EditorApplication.isPlaying || !EditorPrefs.GetBool("LegacyGwent.UseLocalServer."+Application.dataPath,true) || DependencyResolver.Container.Resolve<GwentClientService>().User?.UserName!="premium-ui-test")throw new Exception("Requires local premium-ui-test in Play mode");
            var editor=Resources.FindObjectsOfTypeAll<EditorInfo>().First(x=>x.gameObject.scene.IsValid());
            editor.MainUI.SetActive(true);editor.EditorUI.SetActive(false);
            await DailyQuestClient.Refresh(true);await Task.Delay(400);
            var panel=editor.MainUI.GetComponent<DailyQuestPanel>();
            if(panel==null || !panel.isActiveAndEnabled)throw new Exception("Daily quest UI must update while the main menu is active");
            if(mode=="main")panel.Close();else panel.Open();
            await Task.Delay(700);
            if(DailyQuestClient.State?.Success!=true)throw new Exception("Daily quest sync failed");
            var before=DailyQuestClient.RemainingSeconds;await Task.Delay(1300);
            if(DailyQuestClient.RemainingSeconds>=before-1)throw new Exception("Countdown does not advance locally");
            long balance=PremiumCollectionClient.Account.MeteoritePowder;
            await DailyQuestClient.Refresh(true);
            if(PremiumCollectionClient.Account.MeteoritePowder!=balance)throw new Exception("Repeated refresh paid another login");
            ScreenCapture.CaptureScreenshot(Path.Combine(Work,"ui-"+mode+".png"));
            File.WriteAllText(Path.Combine(Work,"ui-"+mode+".json"),Newtonsoft.Json.JsonConvert.SerializeObject(new{passed=true,state=DailyQuestClient.State,remaining=DailyQuestClient.RemainingSeconds,checks=new[]{"authenticated server synchronization","countdown advances locally","refresh does not duplicate login reward"}},Newtonsoft.Json.Formatting.Indented));
        }
        catch(Exception e){File.WriteAllText(Path.Combine(Work,"ui-error.txt"),e.ToString());Debug.LogException(e);}
        finally{running=false;}
    }
}
