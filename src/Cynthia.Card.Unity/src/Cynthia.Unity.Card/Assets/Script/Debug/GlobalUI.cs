using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalUI : MonoBehaviour
{
    public DebugConsole DebugConsole;
    // Start is called before the first frame update
    void Awake()
    {
        DebugConsole.OnAwake();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.BackQuote))
        {
            DebugConsole.gameObject.SetActive(!DebugConsole.gameObject.activeSelf);
        }
    }
}
