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

    // Private variables to store loaded information
    private Cynthia.Card.CardStatus myLeaderStatus;
    private Cynthia.Card.CardStatus enemyLeaderStatus;
    private string myNameValue = "N/A";
    private string enemyNameValue = "N/A";
    private string myMMRValue = "N/A";
    private string enemyMMRValue = "N/A";
    private string myTitleValue = "N/A";
    private string enemyTitleValue = "N/A";
    
    // Image loading variables
    private bool myAvatarReady = false;
    private bool enemyAvatarReady = false;
    private bool myBorderReady = false;
    private bool enemyBorderReady = false;
    
    // Default sprites to compare against
    private Sprite defaultAvatarSprite;
    private Sprite defaultBorderSprite;

    private void Start()
    {
        // Store default sprites for comparison
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
            bool myLeaderReady = false;
            bool enemyLeaderReady = false;
            bool myNameReady = false;
            bool enemyNameReady = false;
            bool myMMRReady = false;
            bool enemyMMRReady = false;
            bool myTitleReady = false;
            bool enemyTitleReady = false;
            
            // Reset image ready flags
            myAvatarReady = false;
            enemyAvatarReady = false;
            myBorderReady = false;
            enemyBorderReady = false;

            // Check My Leader
            if (MyLeader != null)
            {
                if (MyLeader.TrueCard != null)
                {
                    var myCard = MyLeader.TrueCard.GetComponent<CardShowInfo>();
                    if (myCard != null && myCard.CurrentCore != null)
                    {
                        myLeaderStatus = myCard.CurrentCore;
                        myLeaderReady = true;
                    }
                }
            }
            else
            {
                myLeaderReady = true; // Don't retry if not linked
            }

            // Check Enemy Leader
            if (EnemyLeader != null)
            {
                if (EnemyLeader.TrueCard != null)
                {
                    var enemyCard = EnemyLeader.TrueCard.GetComponent<CardShowInfo>();
                    if (enemyCard != null && enemyCard.CurrentCore != null)
                    {
                        enemyLeaderStatus = enemyCard.CurrentCore;
                        enemyLeaderReady = true;
                    }
                }
            }
            else
            {
                enemyLeaderReady = true; // Don't retry if not linked
            }

            // Check My Name (filter out default "啊啊啊啊啊啊")
            if (MyName != null)
            {
                if (!string.IsNullOrEmpty(MyName.text) && MyName.text != "啊啊啊啊啊啊")
                {
                    myNameValue = MyName.text;
                    myNameReady = true;
                }
            }
            else
            {
                myNameReady = true; // Don't retry if not linked
            }

            // Check Enemy Name (filter out default "啊啊啊啊啊啊")
            if (EnemyName != null)
            {
                if (!string.IsNullOrEmpty(EnemyName.text) && EnemyName.text != "啊啊啊啊啊啊")
                {
                    enemyNameValue = EnemyName.text;
                    enemyNameReady = true;
                }
            }
            else
            {
                enemyNameReady = true; // Don't retry if not linked
            }

            // Check My MMR (filter out default 1000)
            if (MyMMR != null)
            {
                if (!string.IsNullOrEmpty(MyMMR.text) && MyMMR.text != "1000")
                {
                    myMMRValue = MyMMR.text;
                    myMMRReady = true;
                }
            }
            else
            {
                myMMRReady = true; // Don't retry if not linked
            }

            // Check Enemy MMR (filter out default 1000)
            if (EnemyMMR != null)
            {
                if (!string.IsNullOrEmpty(EnemyMMR.text) && EnemyMMR.text != "1000")
                {
                    enemyMMRValue = EnemyMMR.text;
                    enemyMMRReady = true;
                }
            }
            else
            {
                enemyMMRReady = true; // Don't retry if not linked
            }

            // Check My Title (filter out default "CARDSMITHx")
            if (MyTitle != null)
            {
                if (!string.IsNullOrEmpty(MyTitle.text) && MyTitle.text != "CARDSMITHx")
                {
                    myTitleValue = MyTitle.text;
                    myTitleReady = true;
                }
            }
            else
            {
                myTitleReady = true; // Don't retry if not linked
            }

            // Check Enemy Title (filter out default "CARDSMITHx")
            if (EnemyTitle != null)
            {
                if (!string.IsNullOrEmpty(EnemyTitle.text) && EnemyTitle.text != "CARDSMITHx")
                {
                    enemyTitleValue = EnemyTitle.text;
                    enemyTitleReady = true;
                }
            }
            else
            {
                enemyTitleReady = true; // Don't retry if not linked
            }

            // Check My Avatar (wait for it to change from default)
            if (SourceMyAvatar != null)
            {
                if (SourceMyAvatar.sprite != null && SourceMyAvatar.sprite != defaultAvatarSprite)
                {
                    myAvatarReady = true;
                }
            }
            else
            {
                myAvatarReady = true; // Don't retry if not linked
            }

            // Check Enemy Avatar (wait for it to change from default)
            if (SourceEnemyAvatar != null)
            {
                if (SourceEnemyAvatar.sprite != null && SourceEnemyAvatar.sprite != defaultAvatarSprite)
                {
                    enemyAvatarReady = true;
                }
            }
            else
            {
                enemyAvatarReady = true; // Don't retry if not linked
            }

            // Check My Border (wait for it to change from default)
            if (SourceMyBorder != null)
            {
                if (SourceMyBorder.sprite != null && SourceMyBorder.sprite != defaultBorderSprite)
                {
                    myBorderReady = true;
                }
            }
            else
            {
                myBorderReady = true; // Don't retry if not linked
            }

            // Check Enemy Border (wait for it to change from default)
            if (SourceEnemyBorder != null)
            {
                if (SourceEnemyBorder.sprite != null && SourceEnemyBorder.sprite != defaultBorderSprite)
                {
                    enemyBorderReady = true;
                }
            }
            else
            {
                enemyBorderReady = true; // Don't retry if not linked
            }

            // Debug: Print what's ready and what's not
            Debug.Log($"Attempt {attempt}: Leaders={myLeaderReady}/{enemyLeaderReady}, Names={myNameReady}/{enemyNameReady}, MMR={myMMRReady}/{enemyMMRReady}, Titles={myTitleReady}/{enemyTitleReady}, Avatars={myAvatarReady}/{enemyAvatarReady}, Borders={myBorderReady}/{enemyBorderReady}");

            // If all info is ready (or not linked), we're done
            if (myLeaderReady && enemyLeaderReady && myNameReady && enemyNameReady && myMMRReady && enemyMMRReady && myTitleReady && enemyTitleReady && myAvatarReady && enemyAvatarReady && myBorderReady && enemyBorderReady)
            {
                OnAllInfoLoaded();
                yield break;
            }

            // If not ready and we have more attempts, wait and retry
            if (attempt < maxRetries)
            {
                yield return new WaitForSeconds(retryDelay);
            }
        }
    }

    // This method is called when all information is loaded
    private void OnAllInfoLoaded()
    {
        // Get leader IDs for printing only
        string myLeaderId = "N/A";
        string enemyLeaderId = "N/A";
        
        if (myLeaderStatus != null)
        {
            myLeaderId = myLeaderStatus.CardId;
        }
        
        if (enemyLeaderStatus != null)
        {
            enemyLeaderId = enemyLeaderStatus.CardId;
        }
        
        // Print all information in one line
        Debug.Log($"xxxGAME INFO - My: Leader={myLeaderId}, Name={myNameValue}, MMR={myMMRValue}, Title={myTitleValue} | Enemy: Leader={enemyLeaderId}, Name={enemyNameValue}, MMR={enemyMMRValue}, Title={enemyTitleValue}");
        
        // Copy images to target objects
        CopyImages();
        
        // EXAMPLE: Your code goes here
        // This is where you can safely access all game information using the private variables:
        // - myLeaderStatus, enemyLeaderStatus (Cynthia.Card.CardStatus objects)
        // - myNameValue, enemyNameValue  
        // - myMMRValue, enemyMMRValue
        // - myTitleValue, enemyTitleValue
        
        // Your custom logic here...
    }
    
    private void CopyImages()
    {
        Debug.Log("=== COPY IMAGES DEBUG ===");
        
        // Debug source components
        if (SourceMyAvatar != null)
        {
            Debug.Log($"SourceMyAvatar type: {SourceMyAvatar.GetType()}");
            Debug.Log($"SourceMyAvatar sprite: {SourceMyAvatar.sprite}");
            Debug.Log($"SourceMyAvatar sprite type: {SourceMyAvatar.sprite?.GetType()}");
        }
        else
        {
            Debug.Log("SourceMyAvatar is null");
        }
        
        if (SourceMyBorder != null)
        {
            Debug.Log($"SourceMyBorder type: {SourceMyBorder.GetType()}");
            Debug.Log($"SourceMyBorder sprite: {SourceMyBorder.sprite}");
            Debug.Log($"SourceMyBorder sprite type: {SourceMyBorder.sprite?.GetType()}");
        }
        else
        {
            Debug.Log("SourceMyBorder is null");
        }
        
        // Debug target components
        if (MyAvatar != null)
        {
            Debug.Log($"MyAvatar type: {MyAvatar.GetType()}");
        }
        else
        {
            Debug.Log("MyAvatar is null");
        }
        
        if (MyBorder != null)
        {
            Debug.Log($"MyBorder type: {MyBorder.GetType()}");
        }
        else
        {
            Debug.Log("MyBorder is null");
        }
        
        // Copy My Avatar
        if (SourceMyAvatar != null && SourceMyAvatar.sprite != null && MyAvatar != null)
        {
            MyAvatar.sprite = SourceMyAvatar.sprite;
            Debug.Log("Copied My Avatar image");
        }
        else
        {
            Debug.Log("Failed to copy My Avatar - missing components or sprite");
        }
        
        // Copy My Border
        if (SourceMyBorder != null && SourceMyBorder.sprite != null && MyBorder != null)
        {
            MyBorder.sprite = SourceMyBorder.sprite;
            Debug.Log("Copied My Border image");
        }
        else
        {
            Debug.Log("Failed to copy My Border - missing components or sprite");
        }
        
        // Note: Enemy images not copied as requested - only My images for now
        Debug.Log("Image copying completed");
    }
}
