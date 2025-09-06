using UnityEngine;
using Cynthia.Card;
using UnityEngine.UI;

public class StartScreenLogic : MonoBehaviour
{
    [Header("Link these in the Inspector")]
    public LeaderCard MyLeader;
    public LeaderCard EnemyLeader;
    
    [Header("Text Fields (Link these in the Inspector)")]
    public Text MyName;
    public Text EnemyName;
    public Text MyMMR;
    public Text EnemyMMR;
    public Text MyTitle;
    public Text EnemyTitle;
    
    [Header("Source Images (Link these in the Inspector)")]
    public Image SourceMyAvatar;
    public Image SourceEnemyAvatar;
    public Image SourceMyBorder;
    public Image SourceEnemyBorder;
    
    [Header("Target Images (Link these in the Inspector)")]
    public Image MyAvatar;
    public Image MyBorder;
    public Image EnemyAvatar;
    public Image EnemyBorder;

    // Private variables to store loaded information
    private Cynthia.Card.CardStatus myLeaderStatus;
    private Cynthia.Card.CardStatus enemyLeaderStatus;
    private string myNameValue = "N/A";
    private string enemyNameValue = "N/A";
    private string myMMRValue = "N/A";
    private string enemyMMRValue = "N/A";
    private string myTitleValue = "N/A";
    private string enemyTitleValue = "N/A";

    // Default sprites for comparison
    private Sprite defaultAvatarSprite;
    private Sprite defaultBorderSprite;

    private void Start()
    {
        if (SourceMyAvatar != null)
            defaultAvatarSprite = SourceMyAvatar.sprite;
        if (SourceMyBorder != null)
            defaultBorderSprite = SourceMyBorder.sprite;

        StartCoroutine(WaitForAllInfoWithRetry());
    }

    private System.Collections.IEnumerator WaitForAllInfoWithRetry()
    {
        int maxRetries = 6;
        float retryDelay = 0.5f;

        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            bool myLeaderReady = CheckLeader(MyLeader, out myLeaderStatus);
            bool enemyLeaderReady = CheckLeader(EnemyLeader, out enemyLeaderStatus);
            bool myNameReady = CheckText(MyName, ref myNameValue, "啊啊啊啊啊啊");
            bool enemyNameReady = CheckText(EnemyName, ref enemyNameValue, "啊啊啊啊啊啊");
            bool myMMRReady = CheckText(MyMMR, ref myMMRValue, "1000");
            bool enemyMMRReady = CheckText(EnemyMMR, ref enemyMMRValue, "1000");
            bool myTitleReady = CheckText(MyTitle, ref myTitleValue, "CARDSMITHx");
            bool enemyTitleReady = CheckText(EnemyTitle, ref enemyTitleValue, "CARDSMITHx");

            // Check images
            bool myAvatarReady = AssignImageIfReady(SourceMyAvatar, MyAvatar, defaultAvatarSprite);
            bool enemyAvatarReady = AssignImageIfReady(SourceEnemyAvatar, EnemyAvatar, defaultAvatarSprite);
            bool myBorderReady = AssignImageIfReady(SourceMyBorder, MyBorder, defaultBorderSprite);
            bool enemyBorderReady = AssignImageIfReady(SourceEnemyBorder, EnemyBorder, defaultBorderSprite);

            Debug.Log($"Attempt {attempt}: Leaders={myLeaderReady}/{enemyLeaderReady}, Names={myNameReady}/{enemyNameReady}, MMR={myMMRReady}/{enemyMMRReady}, Titles={myTitleReady}/{enemyTitleReady}, Avatars={myAvatarReady}/{enemyAvatarReady}, Borders={myBorderReady}/{enemyBorderReady}");

            if (myLeaderReady && enemyLeaderReady &&
                myNameReady && enemyNameReady &&
                myMMRReady && enemyMMRReady &&
                myTitleReady && enemyTitleReady &&
                myAvatarReady && enemyAvatarReady &&
                myBorderReady && enemyBorderReady)
            {
                OnAllInfoLoaded();
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

    private bool CheckText(Text textField, ref string value, string defaultFilter)
    {
        if (textField == null) return true;
        if (!string.IsNullOrEmpty(textField.text) && textField.text != defaultFilter)
        {
            value = textField.text;
            return true;
        }
        return false;
    }

    private bool AssignImageIfReady(Image source, Image target, Sprite defaultSprite)
    {
        if (source == null || target == null) return true;
        if (source.sprite != null && source.sprite != defaultSprite)
        {
            target.sprite = source.sprite;
            return true;
        }
        return false;
    }

    private void OnAllInfoLoaded()
    {
        string myLeaderId = myLeaderStatus != null ? myLeaderStatus.CardId : "N/A";
        string enemyLeaderId = enemyLeaderStatus != null ? enemyLeaderStatus.CardId : "N/A";

        Debug.Log($"xxxGAME INFO - My: Leader={myLeaderId}, Name={myNameValue}, MMR={myMMRValue}, Title={myTitleValue} | Enemy: Leader={enemyLeaderId}, Name={enemyNameValue}, MMR={enemyMMRValue}, Title={enemyTitleValue}");
    }
}
