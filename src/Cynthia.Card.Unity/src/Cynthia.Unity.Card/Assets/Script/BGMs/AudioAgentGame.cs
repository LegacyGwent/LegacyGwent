using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cynthia.Card;

public class AudioAgentGame : MonoBehaviour
{
    public GameObject editorUI;
    public GameObject matchButton;

    private GameObject tempObject;
    private BGMManager manager;
    // Start is called before the first frame update
    void Update()
    {
        tempObject = GameObject.Find("BGMManager");
        if (tempObject != null)
        {
            manager = tempObject.GetComponent<BGMManager>();
            manager.SetObject(editorUI, 1);
            manager.SetObject(matchButton, 2);
        }
        else
        {
            Debug.Log("Game Agnet 赋值错误！");
        }
    }


}
