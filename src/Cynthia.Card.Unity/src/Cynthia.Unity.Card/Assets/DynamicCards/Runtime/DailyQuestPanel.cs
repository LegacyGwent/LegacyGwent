using System;
using Assets.Script.Localization;
using System.Linq;
using Cynthia.Card;
using UnityEngine;
using UnityEngine.UI;

namespace Assets.Script.DynamicCards
{
    // Original rewards preview: 1600x900, content about 930x562, original backgrounds and reward/crown sprites.
    public sealed class DailyQuestPanel : MonoBehaviour
    {
        private Font font;
        private RectTransform hud, page;
        private Text hudText, timer, balance, login, progress, status;
        private DailyResetRing ring;
        private Image crownFill;
        private readonly Image[] crowns=new Image[3];
        private readonly Text[] tiers=new Text[3];
        private float nextPaint;
        public bool IsOpen => page!=null;
        public static DailyQuestPanel EnsureInitialized(EditorInfo editor)
        {
            if (!ClientContent.HasPremiumContent) return null;
            if(editor==null || editor.MainUI==null)return null;
            var panel=editor.MainUI.GetComponent<DailyQuestPanel>();
            if(panel==null)panel=editor.MainUI.AddComponent<DailyQuestPanel>();
            panel.Initialize(editor);
            return panel;
        }
        public void Initialize(EditorInfo editor)
        {
            if(hud!=null)return;
            font=Resources.Load<Font>("FountInfo/hinted-GWENT-ExtraBold");
            hud=Rect("DailyTasksButton",editor.MainUI.transform,new Vector2(-200,-145),new Vector2(320,90));
            hud.anchorMin=hud.anchorMax=Vector2.one;
            var bg=Image(hud,"Background","current_player_bg",Vector2.zero,new Vector2(320,90));
            var button=hud.gameObject.AddComponent<Button>(); button.targetGraphic=bg; button.onClick.AddListener(Open);
            ring=Rect("ResetRing",hud,new Vector2(-115,0),new Vector2(62,62)).gameObject.AddComponent<DailyResetRing>();
            ring.raycastTarget=false;
            Image(hud,"Clock","timer_preview_icon",new Vector2(-115,0),new Vector2(22,30)).raycastTarget=false;
            hudText=Text(hud,"DailyTasks",new Vector2(30,0),new Vector2(220,78),16);
            DailyQuestClient.Changed+=Paint; PremiumCollectionClient.Changed+=Paint; TextLocalization.LanguageChanged+=Paint;
            Paint();
        }
        public void Open()
        {
            if(page!=null)return;
            page=Rect("DailyQuestPage",null,Vector2.zero,new Vector2(1600,900));
            var canvas=page.gameObject.AddComponent<Canvas>(); canvas.renderMode=RenderMode.ScreenSpaceOverlay;canvas.sortingOrder=200;
            var scaler=page.gameObject.AddComponent<CanvasScaler>();scaler.uiScaleMode=CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution=new Vector2(1600,900);scaler.matchWidthOrHeight=.5f;
            page.gameObject.AddComponent<GraphicRaycaster>();
            var bg=Image(page,"Background","mainmenu_background",Vector2.zero,new Vector2(1600,900));
            bg.rectTransform.anchorMin=Vector2.zero;bg.rectTransform.anchorMax=Vector2.one;bg.rectTransform.offsetMin=bg.rectTransform.offsetMax=Vector2.zero;
            var design=Rect("RewardsPreview",page,Vector2.zero,new Vector2(1600,900));
            foreach(var side in new[]{-1,1})
            {
                var art=Image(design,"SideOrnament",side<0?"ranked_play_season_left_white":"ranked_play_season_right_white",new Vector2(side*550,-24),new Vector2(1000,606));
                art.preserveAspect=true;art.color=new Color(1,1,1,.5f);art.raycastTarget=false;
            }
            LocalizedLabel.Set(Text(design,"Title",new Vector2(0,365),new Vector2(930,70),38),"DailyQuest_Title");
            timer=Text(design,"ResetCountdown",new Vector2(0,305),new Vector2(930,45),21);
            var loginBg=Image(design,"LoginReward","current_player_bg",new Vector2(0,200),new Vector2(930,110));
            LocalizedLabel.Set(Text(loginBg.transform,"Title",new Vector2(-260,15),new Vector2(320,40),25),"DailyQuest_LoginTitle");
            LocalizedLabel.Set(Text(loginBg.transform,"Description",new Vector2(-220,-24),new Vector2(400,32),18),"DailyQuest_LoginDescription");
            var powder=Image(loginBg.transform,"Powder",null,new Vector2(215,0),new Vector2(48,48));powder.sprite=Resources.Load<Sprite>("PremiumCrafting/Powder");
            login=Text(loginBg.transform,"LoginStatus",new Vector2(345,0),new Vector2(220,80),22);
            LocalizedLabel.Set(Text(design,"CrownTitle",new Vector2(-260,88),new Vector2(410,50),28),"DailyQuest_RoundsTitle");
            progress=Text(design,"CrownCount",new Vector2(290,88),new Vector2(350,50),23);
            var bar=Image(design,"ProgressBar","Progression_bar_bg",new Vector2(0,-35),new Vector2(910,35));bar.raycastTarget=false;
            crownFill=Image(design,"ProgressFill","Progression_bar_fill",new Vector2(0,-35),new Vector2(900,29));
            crownFill.type=UnityEngine.UI.Image.Type.Filled;crownFill.fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;crownFill.fillOrigin=0;crownFill.raycastTarget=false;
            for(int i=0;i<3;i++)
            {
                var empty=Image(design,"Crown"+i,"crown-empty",new Vector2(-300+i*300,8),new Vector2(60,60));
                empty.color=new Color(.6f,.6f,.6f);empty.raycastTarget=false;
                crowns[i]=Image(empty.transform,"EarnedHalf","Crown",Vector2.zero,new Vector2(60,60));
                crowns[i].type=UnityEngine.UI.Image.Type.Filled;
                crowns[i].fillMethod=UnityEngine.UI.Image.FillMethod.Horizontal;
                crowns[i].fillOrigin=0;crowns[i].fillAmount=0;crowns[i].raycastTarget=false;
            }
            for(int i=1;i<=6;i++)
            {
                float x=-450+i*150;
                var tick=Image(design,"ProgressTick"+i,null,new Vector2(x,-35),new Vector2(2,25));
                tick.color=new Color(.92f,.8f,.48f,.9f);tick.raycastTarget=false;
                Text(design,"ProgressStep"+i,new Vector2(x,-62),new Vector2(32,20),14).text=i.ToString();
            }
            for(int i=0;i<3;i++)
            {
                float x=-310+i*310;
                var card=Image(design,"Tier"+i,"current_player_bg",new Vector2(x,-135),new Vector2(295,120));
                tiers[i]=Text(card.transform,"Reward",Vector2.zero,new Vector2(285,112),22);
            }
            status=Text(design,"QuestStatus",new Vector2(0,-235),new Vector2(950,55),20);
            balance=Text(design,"Wallet",new Vector2(0,-305),new Vector2(930,40),22);
            var close=Rect("CloseDailyQuests",design,new Vector2(0,-400),new Vector2(240,50));
            var click=close.gameObject.AddComponent<Image>();click.color=Color.clear;
            var closeButton=close.gameObject.AddComponent<Button>();closeButton.targetGraphic=click;closeButton.onClick.AddListener(Close);
            LocalizedLabel.Set(Text(close,"Label",Vector2.zero,new Vector2(240,50),24),"Common_CloseEsc");
            _=DailyQuestClient.Refresh(true);Paint();
        }
        public void Close() { if(page!=null)Destroy(page.gameObject);page=null; }
        private void Update()
        {
            if(page!=null && Input.GetKeyDown(KeyCode.Escape))Close();
            if(Time.realtimeSinceStartup<nextPaint)return;nextPaint=Time.realtimeSinceStartup+1;Paint();
        }
        private void Paint()
        {
            if(hud==null)return;
            var state=DailyQuestClient.State;
            var daily=PremiumCollectionClient.Account?.DailyQuests;
            var left=TimeSpan.FromSeconds(DailyQuestClient.RemainingSeconds);
            string countdown=state==null?LocalizedLabel.Get("DailyQuest_Syncing"):left.TotalSeconds<=0?LocalizedLabel.Get("DailyQuest_ResetPending"):LocalizedLabel.Get("DailyQuest_Countdown",(int)left.TotalHours,left.Minutes);
            hudText.text=LocalizedLabel.Get("DailyQuest_Title")+"\n"+(state==null?LocalizedLabel.Get("Common_Syncing"):LocalizedLabel.Get("DailyQuest_HudProgress",(daily?.Crowns??0)/2f,countdown));
            ring.Value=state==null?0:(float)(DailyQuestClient.RemainingSeconds/86400d);
            if(page==null)return;
            timer.text=LocalizedLabel.Get("DailyQuest_ResetTime",countdown);
            if(state==null || daily==null)
            {login.text=LocalizedLabel.Get("Common_Syncing");progress.text=LocalizedLabel.Get("DailyQuest_Progress","—",3);crownFill.fillAmount=0;foreach(var crown in crowns)crown.fillAmount=0;foreach(var tier in tiers)tier.text="";balance.text="";status.text=DailyQuestClient.Error??LocalizedLabel.Get("DailyQuest_Fetching");return;}
            login.text=LocalizedLabel.Get("DailyQuest_LoginReward",state.LoginPowder,LocalizedLabel.Get(daily.LoginGranted?"DailyQuest_Granted":"DailyQuest_Incomplete"));
            int cap=state.Tiers.Last().Crowns;
            crownFill.fillAmount=daily.Crowns/(float)cap;
            progress.text=LocalizedLabel.Get("DailyQuest_Progress",daily.Crowns/2f,cap/2f);
            for(int i=0;i<crowns.Length;i++)
            {crowns[i].fillAmount=Mathf.Clamp01((daily.Crowns-i*2)/2f);}
            for(int i=0;i<tiers.Length;i++)
            {var tier=state.Tiers[i];tiers[i].text=LocalizedLabel.Get("DailyQuest_Tier",tier.Crowns/2f,tier.Powder,LocalizedLabel.Get(daily.Crowns>=tier.Crowns?"DailyQuest_Granted":"DailyQuest_Incomplete"));tiers[i].color=daily.Crowns>=tier.Crowns?new Color(.92f,.8f,.48f):Color.white;}
            status.text=DailyQuestClient.Error??LocalizedLabel.Get(daily.Crowns>=cap?"DailyQuest_Complete":"DailyQuest_Rules");
            balance.text=LocalizedLabel.Get("DailyQuest_Wallet",daily.PowderGranted,state.DailyCap,PremiumCollectionClient.Account.MeteoritePowder);
        }
        private void OnDisable(){Close();}
        private void OnDestroy(){DailyQuestClient.Changed-=Paint;PremiumCollectionClient.Changed-=Paint;TextLocalization.LanguageChanged-=Paint;Close();}
        private static RectTransform Rect(string name,Transform parent,Vector2 pos,Vector2 size)=>PremiumCollectionPanel.Rect(name,parent,new Vector2(.5f,.5f),pos,size);
        private static Image Image(Transform parent,string name,string sprite,Vector2 pos,Vector2 size)
        {var image=Rect(name,parent,pos,size).gameObject.AddComponent<Image>();if(sprite!=null)image.sprite=Resources.Load<Sprite>("DailyQuests/"+sprite);return image;}
        private Text Text(Transform parent,string name,Vector2 pos,Vector2 size,int fontSize)
        {var text=Rect(name,parent,pos,size).gameObject.AddComponent<Text>();text.font=font;text.fontSize=fontSize;text.resizeTextForBestFit=true;text.resizeTextMinSize=Mathf.Max(11,fontSize-5);text.resizeTextMaxSize=fontSize;text.alignment=TextAnchor.MiddleCenter;text.color=Color.white;text.raycastTarget=false;return text;}
    }
    public sealed class DailyResetRing : MaskableGraphic
    {
        private float value;
        public float Value {set{if(Mathf.Abs(this.value-value)<.0001f)return;this.value=value;SetVerticesDirty();}}
        protected override void OnPopulateMesh(VertexHelper vh)
        {
            vh.Clear();float radius=Mathf.Min(rectTransform.rect.width,rectTransform.rect.height)*.5f;
            for(int i=0;i<64;i++)
            {
                float a=Mathf.PI*.5f-i*Mathf.PI*2/64,b=Mathf.PI*.5f-(i+1)*Mathf.PI*2/64;
                Color shade=i/64f<value?new Color(.84f,.70f,.37f):new Color(.25f,.23f,.19f);
                int n=vh.currentVertCount;
                foreach(var point in new[]{new Vector2(Mathf.Cos(a),Mathf.Sin(a))*radius,new Vector2(Mathf.Cos(b),Mathf.Sin(b))*radius,new Vector2(Mathf.Cos(b),Mathf.Sin(b))*(radius-3),new Vector2(Mathf.Cos(a),Mathf.Sin(a))*(radius-3)})vh.AddVert(point,shade,Vector2.zero);
                vh.AddTriangle(n,n+1,n+2);vh.AddTriangle(n,n+2,n+3);
            }
        }
    }
}
