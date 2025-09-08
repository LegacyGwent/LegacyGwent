using UnityEngine;
using Cynthia.Card;
using UnityEngine.UI;
using TMPro;
using Assets.Script.Localization;
using Autofac;
using System.Collections.Generic;

public class StartScreenLogic : MonoBehaviour
{

    private LocalizationService translator;

    [Header("Scripts")]
    public MyCards MyCards;


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
    public TextMeshProUGUI EnemyyLeaderNameField;
    public TextMeshProUGUI EnemyLeaderTagsField;

    [Header("Source Images")]
    public Image SourceMyAvatar;
    public Image SourceEnemyAvatar;
    public Image SourceMyBorder;
    public Image SourceEnemyBorder;

    [Header("Target Images")]
    public Image MyAvatarTarget;
    public Image MyBorderTarget;
    public Image MyFactionBackground;

    public Image EnemyAvatarTarget;
    public Image EnemyBorderTarget;
    public Image EnemyFactionBackground;

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
        init();
        StartCoroutine(WaitForAllInfoWithRetry());
    }
    public void init()
    {
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    private System.Collections.IEnumerator WaitForAllInfoWithRetry()
    {
        int maxRetries = 6;
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
                ApplyDataToUI(); // only my fields/images
                yield break;
            }

            if (attempt < maxRetries)
                yield return new WaitForSeconds(retryDelay);
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
        MyNameField.text = myNameValue;
        MyMMRField.text = myMMRValue;
        MyTitleField.text = myTitleValue;

        MyLeaderNameField.text=translator.GetCardName(myLeaderStatus.CardId);
        MyLeaderTagsField.text=TagToString(myLeaderStatus);

        MyAvatarTarget.sprite = loadedMyAvatar;
        MyBorderTarget.sprite = loadedMyBorder;
        SetBackground(myLeaderStatus,MyFactionBackground);


        MyCards.SetCard(myLeaderStatus.CardId);
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
    public void SetBackground(CardStatus Card, Image target)
    {
        switch (GwentMap.CardMap[myLeaderStatus.CardId].Faction)
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
}
