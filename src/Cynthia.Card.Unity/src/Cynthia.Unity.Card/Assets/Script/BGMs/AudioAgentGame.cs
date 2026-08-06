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
    private IEnumerator Start()
    {
        while (manager == null)
        {
            tempObject = GameObject.Find("BGMManager");
            if (tempObject != null)
            {
                manager = tempObject.GetComponent<BGMManager>();
            }

            if (manager == null)
            {
                yield return new WaitForSeconds(1f);
            }
        }

        manager.SetObject(editorUI, 1);
        manager.SetObject(matchButton, 2);
    }

}
