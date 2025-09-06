using UnityEngine;
using Cynthia.Card;
using UnityEngine.UI;
using TMPro;
using Assets.Script.Localization;
using Autofac;

public class StartScreenLogic : MonoBehaviour
{

    private LocalizationService translator;


    [Header("Link these in the Inspector")]
    public LeaderCard MyLeader;
    public LeaderCard EnemyLeader;

    [Header("Read Fields (source of truth)")]
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
    public Image EnemyAvatarTarget;
    public Image EnemyBorderTarget;

    // Private loaded info
    private CardStatus myLeaderStatus;
    private CardStatus enemyLeaderStatus;

    private string myNameValue = "N/A";
    private string myMMRValue = "N/A";
    private string myTitleValue = "N/A";

    private string enemyNameValue = "N/A";
    private string enemyMMRValue = "N/A";
    private string enemyTitleValue = "N/A";

    // Loaded sprites (not yet applied to UI)
    private Sprite loadedMyAvatar;
    private Sprite loadedEnemyAvatar;
    private Sprite loadedMyBorder;
    private Sprite loadedEnemyBorder;

    private void Start()
    {
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
        StartCoroutine(WaitForAllInfoWithRetry());
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
        
        Debug.Log(translator.GetCardName(myLeaderStatus.CardId));

        MyLeaderNameField.text=translator.GetCardName(myLeaderStatus.CardId);


        string tagtext="";
        if (myLeaderStatus.Categories.Length > 0)
        {
            foreach (Categorie categorie in myLeaderStatus.Categories)
            {
                tagtext=tagtext+translator.GetText($"CardTag_"+categorie)+", ";
            }
            tagtext = tagtext.Remove(tagtext.Length - 2);
        }
        MyLeaderTagsField.text=tagtext;


        MyAvatarTarget.sprite = loadedMyAvatar;
        MyBorderTarget.sprite = loadedMyBorder;

        // Enemy fields are read internally but NOT applied
    }
}
