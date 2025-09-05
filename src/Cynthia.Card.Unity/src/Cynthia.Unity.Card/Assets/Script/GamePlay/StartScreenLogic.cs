using UnityEngine;
using Cynthia.Card;

public class StartScreenLogic : MonoBehaviour
{
    [Header("Link these in the Inspector")]
    public LeaderCard MyLeader;
    public LeaderCard EnemyLeader;

    private void Start()
    {
        StartCoroutine(LogLeaderInfoWithRetry());
    }

    private System.Collections.IEnumerator LogLeaderInfoWithRetry()
    {
        int maxRetries = 10;
        float retryDelay = 0.5f;
        
        //Debug.Log("SimpleLeaderDebug: Starting retry loop");
        
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            //Debug.Log($"Attempt {attempt}: MyLeader={MyLeader != null}, EnemyLeader={EnemyLeader != null}");
            
            bool myLeaderReady = false;
            bool enemyLeaderReady = false;

            // Check My Leader
            if (MyLeader != null)
            {
                //Debug.Log($"MyLeader TrueCard: {MyLeader.TrueCard != null}");
                if (MyLeader.TrueCard != null)
                {
                    var myCard = MyLeader.TrueCard.GetComponent<CardShowInfo>();
                    //Debug.Log($"MyLeader CardShowInfo: {myCard != null}");
                    if (myCard != null)
                    {
                        Debug.Log($"MY LEADER - ID: {myCard.CurrentCore.CardId}");
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
                //Debug.Log($"EnemyLeader TrueCard: {EnemyLeader.TrueCard != null}");
                if (EnemyLeader.TrueCard != null)
                {
                    var enemyCard = EnemyLeader.TrueCard.GetComponent<CardShowInfo>();
                    //Debug.Log($"EnemyLeader CardShowInfo: {enemyCard != null}");
                    if (enemyCard != null)
                    {
                        Debug.Log($"ENEMY LEADER - ID: {enemyCard.CurrentCore.CardId}");
                        enemyLeaderReady = true;
                    }
                }
            }
            else
            {
                enemyLeaderReady = true; // Don't retry if not linked
            }

            // If both leaders are ready (or not linked), we're done
            if (myLeaderReady && enemyLeaderReady)
            {
                //Debug.Log("Both leaders ready, stopping");
                OnLeadersLoaded();
                yield break;
            }

            // If not ready and we have more attempts, wait and retry
            if (attempt < maxRetries)
            {
                yield return new WaitForSeconds(retryDelay);
            }
        }
        
        //Debug.Log("Max retries reached");
    }

    // This method is called when both leaders are loaded
    private void OnLeadersLoaded()
    {
        Debug.Log("=== LEADERS LOADED - START YOUR CODE HERE ===");
        
        // EXAMPLE: Your code goes here
        // This is where you can safely access leader information
        // Example:
        // - Initialize UI elements
        // - Set up game logic
        // - Start animations
        // - etc.
        
        // Example code:
        Debug.Log("Now you can safely work with leader data!");
        // Your custom logic here...
    }
}
