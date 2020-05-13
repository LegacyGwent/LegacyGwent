using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card;

public class AudioAgentGameplay : MonoBehaviour
{
    //public CardStatus cardStat = null;
    public GameObject enenmyLeader;
    public GameObject smallRoundUI;
    //public GameObject bigRound;
    public GameObject resultUI;
    public GameObject resultPage;

    private GameObject tempObject;
    private BGMManager manager;
    // Start is called before the first frame update
    void Start()
    {
        tempObject = GameObject.Find("BGMManager");
        if (tempObject != null)
        {
            manager = tempObject.GetComponent<BGMManager>();
            manager.SetObject(enenmyLeader, 3);
            manager.SetObject(smallRoundUI, 4);
            manager.SetObject(resultUI, 5);
            manager.SetObject(resultPage, 6);

        }
        else
        {
            Debug.Log("Gmaeplay Agnet 赋值错误！");
        }
    }
}
