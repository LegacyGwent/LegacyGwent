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

    // Private variables to store loaded information
    private string myLeaderId = "N/A";
    private string enemyLeaderId = "N/A";
    private string myNameValue = "N/A";
    private string enemyNameValue = "N/A";
    private string myMMRValue = "N/A";
    private string enemyMMRValue = "N/A";
    private string myTitleValue = "N/A";
    private string enemyTitleValue = "N/A";

    private void Start()
    {
        StartCoroutine(WaitForAllInfoWithRetry());
    }

    private System.Collections.IEnumerator WaitForAllInfoWithRetry()
    {
        int maxRetries = 20;
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

            // Check My Leader
            if (MyLeader != null)
            {
                if (MyLeader.TrueCard != null)
                {
                    var myCard = MyLeader.TrueCard.GetComponent<CardShowInfo>();
                    if (myCard != null)
                    {
                        myLeaderId = myCard.CurrentCore.CardId;
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
                    if (enemyCard != null)
                    {
                        enemyLeaderId = enemyCard.CurrentCore.CardId;
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

            // If all info is ready (or not linked), we're done
            if (myLeaderReady && enemyLeaderReady && myNameReady && enemyNameReady && myMMRReady && enemyMMRReady && myTitleReady && enemyTitleReady)
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
        // Print all information in one line
        Debug.Log($"GAME INFO - My: Leader={myLeaderId}, Name={myNameValue}, MMR={myMMRValue}, Title={myTitleValue} | Enemy: Leader={enemyLeaderId}, Name={enemyNameValue}, MMR={enemyMMRValue}, Title={enemyTitleValue}");
        
        // EXAMPLE: Your code goes here
        // This is where you can safely access all game information using the private variables:
        // - myLeaderId, enemyLeaderId
        // - myNameValue, enemyNameValue  
        // - myMMRValue, enemyMMRValue
        // - myTitleValue, enemyTitleValue
        
        // Your custom logic here...
    }
}
