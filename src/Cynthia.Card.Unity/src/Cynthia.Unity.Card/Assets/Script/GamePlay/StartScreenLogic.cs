using UnityEngine;
using Cynthia.Card;
using UnityEngine.UI;
using TMPro;
using Assets.Script.Localization;
using Autofac;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System;
using System.Collections;
using UnityEngine.EventSystems;

public class StartScreenLogic : MonoBehaviour
{

    private LocalizationService translator;

    [Header("Scripts")]
    public EventSystem eventSystem;

    public MyCards MyCards;
    public MyCards EnemyCards;
    public StartingInfo MyInfo;
    public StartingInfo EnemyInfo;
    public Animator MyBackground;
    public Animator EnemyBackground;
    public Animator WholeBackground;
    public Animator VS;
    [Header("Read Fields (source of truth)")]
    public LeaderCard MyLeader;
    public LeaderCard EnemyLeader;
    public Text MyNameReadField;
    public Text MyMMRReadField;
    public Text MyTitleReadField;
    public Text EnemyNameReadField;
    public Text EnemyMMRReadField;
    public Text EnemyTitleReadField;

    [Header("TextMeshPro Fields (where we set values)")]
    public TextMeshProUGUI MyNameField;
    public TextMeshProUGUI MyMMRField;
    public TextMeshProUGUI MyTitleField;
    public TextMeshProUGUI MyLeaderNameField;
    public TextMeshProUGUI MyLeaderTagsField;

    public TextMeshProUGUI EnemyNameField;
    public TextMeshProUGUI EnemyMMRField;
    public TextMeshProUGUI EnemyTitleField;
    public TextMeshProUGUI EnemyLeaderNameField;
    public TextMeshProUGUI EnemyLeaderTagsField;

    public TextMeshProUGUI LoadingTextField;

    [Header("Source Images")]
    public Image SourceMyAvatar;
    public Image SourceEnemyAvatar;
    public Image SourceMyBorder;
    public Image SourceEnemyBorder;

    [Header("Target Images")]
    public Image MyAvatarTarget;
    public Image MyBorderTarget;
    public Image MyFactionBackground;
    public Image MyRank;

    public Image EnemyAvatarTarget;
    public Image EnemyBorderTarget;
    public Image EnemyFactionBackground;
    public Image EnemyRank;

    [Header("Bacground Fields")]
    public Image MyLeft;
    public Image MyRight;
    public Image EnemyLeft;
    public Image EnemyRight;


    [Header("Card Sprites")]
    public Sprite NorthernRealmsContent;
    public Sprite ScoiaTaelContent;
    public Sprite MonstersContent;
    public Sprite SkelligeContent;
    public Sprite NilfgaardContent;
    public Sprite NeutralContent;

    [Header("BackGround Sprites")]
    public Sprite NeutralLeft;
    public Sprite NeutralRight;
    public Sprite SKLeft;
    public Sprite SKRight;
    public Sprite STLeft;
    public Sprite STRight;
    public Sprite NGLeft;
    public Sprite NGRight;
    public Sprite NRLeft;
    public Sprite NRRight;
    public Sprite MOLeft;
    public Sprite MORight;




    // Private loaded info
    private CardStatus myLeaderStatus;
    private CardStatus enemyLeaderStatus;

    private string myNameValue = "N/A";
    private string myMMRValue = "N/A";
    private string myTitleValue = "N/A";

    private string enemyNameValue = "N/A";
    private string enemyMMRValue = "N/A";
    private string enemyTitleValue = "N/A";

    // Loaded sprites
    private Sprite loadedMyAvatar;
    private Sprite loadedEnemyAvatar;
    private Sprite loadedMyBorder;
    private Sprite loadedEnemyBorder;

    private void Start()
    {
        DisableMouseInput();
        init();
        StartCoroutine(WaitForAllInfoWithRetry());
        //StartCoroutine(PlayAnimations());
    }
    public void init()
    {
        
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
        LoadingTextField.text = translator.GetText("Loading_game_info");
    }
    private void Awake()
    {
        // Try to find EventSystem automatically
        eventSystem = EventSystem.current;

        if (eventSystem == null)
        {
            Debug.LogWarning("No EventSystem found in the scene!");
        }
    }
    private System.Collections.IEnumerator WaitForAllInfoWithRetry()
    {
        int maxRetries = 16;
        float retryDelay = 0.5f;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            // Read leaders
            bool myLeaderReady = CheckLeader(MyLeader, out myLeaderStatus);
            bool enemyLeaderReady = CheckLeader(EnemyLeader, out enemyLeaderStatus);

            // Read from source fields
            bool myNameReady = ReadField(MyNameReadField, ref myNameValue, "啊啊啊啊啊啊");
            bool myMMRReady = ReadField(MyMMRReadField, ref myMMRValue, "1000");
            bool myTitleReady = ReadField(MyTitleReadField, ref myTitleValue, "CARDSMITHx");

            bool enemyNameReady = ReadField(EnemyNameReadField, ref enemyNameValue, "啊啊啊啊啊啊");
            bool enemyMMRReady = ReadField(EnemyMMRReadField, ref enemyMMRValue, "1000");
            bool enemyTitleReady = ReadField(EnemyTitleReadField, ref enemyTitleValue, "CARDSMITHx");

            // Load images into private variables
            bool myAvatarReady = LoadImage(SourceMyAvatar, ref loadedMyAvatar);
            bool myBorderReady = LoadImage(SourceMyBorder, ref loadedMyBorder);
            bool enemyAvatarReady = LoadImage(SourceEnemyAvatar, ref loadedEnemyAvatar);
            bool enemyBorderReady = LoadImage(SourceEnemyBorder, ref loadedEnemyBorder);

            Debug.Log($"Attempt {attempt}: Leaders={myLeaderReady}/{enemyLeaderReady}, MyDataReady={myNameReady && myMMRReady && myTitleReady}, EnemyDataReady={enemyNameReady && enemyMMRReady && enemyTitleReady}, ImagesReady={myAvatarReady && myBorderReady && enemyAvatarReady && enemyBorderReady}");

            // Apply once everything loaded
            if (myLeaderReady && enemyLeaderReady &&
                myNameReady && myMMRReady && myTitleReady &&
                enemyNameReady && enemyMMRReady && enemyTitleReady &&
                myAvatarReady && myBorderReady &&
                enemyAvatarReady && enemyBorderReady)
            {
                Debug.Log("All data loaded, applying my UI elements now.");
                LoadingTextField.gameObject.SetActive(false);
                ApplyDataToUI();
                yield break;
            }

            if (attempt < maxRetries)
                yield return new WaitForSeconds(retryDelay);
            if (attempt >= maxRetries)
            {
                StartCoroutine(CloseAfterSeconds(0f));
                EnableMouseInput();
            }
        }
    }

    private bool CheckLeader(LeaderCard leader, out CardStatus status)
    {
        status = null;
        if (leader == null) return true;

        if (leader.TrueCard != null)
        {
            var cardShowInfo = leader.TrueCard.GetComponent<CardShowInfo>();
            if (cardShowInfo != null && cardShowInfo.CurrentCore != null)
            {
                status = cardShowInfo.CurrentCore;
                return true;
            }
        }
        return false;
    }

    private bool ReadField(Text sourceField, ref string targetValue, string defaultFilter)
    {
        if (sourceField == null) return true;

        targetValue = sourceField.text;
        return !string.IsNullOrEmpty(targetValue) && targetValue != defaultFilter;
    }

    private bool LoadImage(Image source, ref Sprite targetVar)
    {
        if (source == null) return true;
        if (source.sprite != null && source.sprite != null)
        {
            targetVar = source.sprite;
            return true;
        }
        return false;
    }

    // Apply loaded data to TMP fields (my elements only)
    public void ApplyDataToUI()
    {
        //my
        MyNameField.text = myNameValue;
        MyMMRField.text = myMMRValue;
        MyTitleField.text = myTitleValue;
        MyLeaderNameField.text=translator.GetCardName(myLeaderStatus.CardId);
        MyLeaderTagsField.text=TagToString(myLeaderStatus);
        MyAvatarTarget.sprite = loadedMyAvatar;
        MyBorderTarget.sprite = loadedMyBorder;
        SetTextBackground(myLeaderStatus,MyFactionBackground);
        SetBackground(myLeaderStatus,MyLeft,MyRight);
        MyCards.SetCard(myLeaderStatus.CardId);
        SetRank(myMMRValue,MyRank);
        //Enemy
        EnemyNameField.text = enemyNameValue; //works
        EnemyMMRField.text = enemyMMRValue; //works
        EnemyTitleField.text = enemyTitleValue; //works
        EnemyLeaderNameField.text=translator.GetCardName(enemyLeaderStatus.CardId);//works
        EnemyLeaderTagsField.text=TagToString(enemyLeaderStatus);//works
        EnemyAvatarTarget.sprite = loadedEnemyAvatar;//works
        EnemyBorderTarget.sprite = loadedEnemyBorder;//works
        SetTextBackground(enemyLeaderStatus,EnemyFactionBackground);//works
        SetBackground(enemyLeaderStatus,EnemyLeft,EnemyRight);
        EnemyCards.SetCard(enemyLeaderStatus.CardId);//works
        SetRank(enemyMMRValue,EnemyRank);//?

        StartCoroutine(PlayAnimations());
    }




    public string TagToString(CardStatus CardStatus)
    {
        string tagtext="";
        if (CardStatus.Categories.Length > 0)
        {
            foreach (Categorie categorie in CardStatus.Categories)
            {
                tagtext=tagtext+translator.GetText($"CardTag_"+categorie)+", ";
            }
            tagtext = tagtext.Remove(tagtext.Length - 2);
        }
        return tagtext;
    }
    public void SetTextBackground(CardStatus Card, Image target)
    {
        switch (GwentMap.CardMap[Card.CardId].Faction)
        {
            case Faction.Monsters:
                target.sprite = MonstersContent;
                break;
            case Faction.Nilfgaard:
                target.sprite = NilfgaardContent;
                break;
            case Faction.NorthernRealms:
                target.sprite = NorthernRealmsContent;
                break;
            case Faction.ScoiaTael:
                target.sprite = ScoiaTaelContent;
                break;
            case Faction.Skellige:
                target.sprite = SkelligeContent;
                break;
            case Faction.Neutral:
                target.sprite = NeutralContent;
                break;
        }
    }
    public void SetBackground(CardStatus Card, Image Left, Image Right)
    {
        Debug.Log($"Faction: {GwentMap.CardMap[Card.CardId].Faction}");
        switch (GwentMap.CardMap[Card.CardId].Faction)
        {
            case Faction.Monsters:
                Left.sprite = MOLeft;
                Right.sprite = MORight;
                break;
            case Faction.Nilfgaard:
                Left.sprite = NGLeft;
                Right.sprite = NGRight;
                break;
            case Faction.NorthernRealms:
                Left.sprite = NRLeft;
                Right.sprite = NRRight;
                break;
            case Faction.ScoiaTael:
                Left.sprite = STLeft;
                Right.sprite = STRight;
                break;
            case Faction.Skellige:
                Left.sprite = SKLeft;
                Right.sprite = SKRight;
                break;
            case Faction.Neutral:
                Left.sprite = NeutralLeft;
                Right.sprite = NeutralRight;
                break;
        }
    }
    public void SetRank(string MMR, Image myRank)
    {
        int mmrInt = int.Parse(MMR);
        int rank = Math.Min(Math.Max((mmrInt - 3400) / 50, 1), 21);
        var op = Addressables.LoadAssetAsync<Sprite>($"rank_{rank}");
        Sprite sprite = op.WaitForCompletion();
        myRank.sprite = sprite;
    }

    private System.Collections.IEnumerator CloseAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }

    private IEnumerator PlayAnimations()
    {
        Debug.Log("Start Animations");
        //yield return new WaitForSeconds(1f); 
        WholeBackground.Play("LineFadeIn", 0, 0f);
        EnemyBackground.Play("EnemyBackgroundFadeIn", 0, 0f);
        EnemyCards.EnemyCardMoveIn();
        EnemyInfo.FadeIn();
        bool EnemyPlayedAudio=AudioManager.Instance.PlayAudio(GwentMap.CardMap[enemyLeaderStatus.CardId].CardArtsId, AudioType.Card, AudioPlayMode.Append);
        yield return new WaitForSeconds(2f); 
        VS.Play("VS_In", 0, 0f);
        yield return new WaitForSeconds(0.5f); 
        MyBackground.Play("MyBackgroundFadeIn", 0, 0f);
        MyCards.MyCardMoveIn();
        MyInfo.FadeIn();
        bool MinePlayedAudio=AudioManager.Instance.PlayAudio(GwentMap.CardMap[myLeaderStatus.CardId].CardArtsId, AudioType.Card, AudioPlayMode.Append);
        if (!EnemyPlayedAudio && !MinePlayedAudio) //plays horn if both leaders are silent
        {
            AudioManager.Instance.PlayAudio(GwentMap.CardMap["13027"].CardArtsId, AudioType.Card, AudioPlayMode.Append);
        }
        yield return new WaitForSeconds(3f); 
        EnemyCards.EnemyCardMoveOut();
        MyCards.MyCardMoveOut();
        WholeBackground.Play("BackgroundFadeOut", 0, 0f);
        MyInfo.FadeOut();
        EnemyInfo.FadeOut();
        yield return new WaitForSeconds(0.5f); 
        EnableMouseInput();
        yield return new WaitForSeconds(5f); 
    }

    public void DisableMouseInput()
    {
        if (eventSystem != null)
            eventSystem.enabled = false;
    }

    // Enable mouse input
    public void EnableMouseInput()
    {
        if (eventSystem != null)
            eventSystem.enabled = true;
    }
}
 