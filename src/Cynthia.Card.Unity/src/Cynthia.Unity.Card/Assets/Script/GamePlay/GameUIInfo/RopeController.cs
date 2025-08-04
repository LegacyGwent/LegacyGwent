using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using System.Timers;
using Cynthia.Card.Client;
using Autofac;

public class RopeController : MonoBehaviour
{
    public Slider rope;
    public Text TimeCount;
    public GameObject WolfIcon;
    private float ropeTime = 20f;
    private float endWaitTime = 0.5f;
    private int shakeRange = 1;
    public bool isRunning = false;
    public float remainingTime = 90f;
    public int skipedTurns=0;
    private int count = 0; // count for fixed update to record if it's the first time it is called during a turn
    private int oldCount = 0;
    public async void Surrender()
    {
        await DependencyResolver.Container.Resolve<GwentClientService>().Surrender();
    }
    public void StartRopeTimer(float totalTime = 90f)
    {
        rope.gameObject.SetActive(false); // is not active until ropeTime
        remainingTime = totalTime;
        isRunning = true;
    }
    public void StopRopeTimer(int count)
    {
        GameEvent gameEvent = FindObjectOfType<GameEvent>();


        // Check if it's the local player's turn by checking the coin state
        bool isLocalPlayerTurn = gameEvent.Coin.IsMyRound;
        if (isLocalPlayerTurn && count != (oldCount +1))
        {
            // If it's the local player's turn and this is the first time the rope timer is called during this turn
            // We assume that the local player has triggered the timeout
            // This is to prevent the rope timer from being triggered multiple times in a single turn
            Debug.Log("Local player triggered the rope timer timeout");
            skipedTurns = skipedTurns + 1;
            Debug.Log("Skipped Turns: " + skipedTurns);
            gameEvent.shorterTimer = true;
        }
        Debug.Log("TIMED-OUT");

        if (skipedTurns >= 3)
        {
            Surrender();
        }
        rope.gameObject.SetActive(false);
        remainingTime = 0;
        isRunning = false;
        oldCount = count;
    }
    private void FixedUpdate()
    {
        if (!isRunning)
        {
            return;
        }
        count += 1;
        remainingTime -= Time.deltaTime;
        if (remainingTime < ropeTime + endWaitTime)
        {
            // active rope
            if (!rope.gameObject.activeSelf)
            {
                rope.maxValue = remainingTime - endWaitTime;
                rope.gameObject.SetActive(true);
            }
            rope.value = remainingTime - endWaitTime;
            TimeCount.text = ((int)(rope.value + 0.5)).ToString(); // add 0.5 to round
        }
        if (remainingTime <= 0)
        {
            StopRopeTimer(count);
        }
    }
    void Update()
    {
        WolfIcon.transform.eulerAngles = new Vector3(0, 0, Random.Range(-shakeRange, shakeRange));
    }
}
