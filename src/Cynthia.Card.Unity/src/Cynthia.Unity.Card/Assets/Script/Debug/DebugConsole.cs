using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugConsole : MonoBehaviour
{
    public Text Text;
    public Scrollbar Scrollbar;
    public RectTransform Content;
    private int _line = 0;
    // Start is called before the first frame update
    public void OnAwake()
    {
        Application.logMessageReceived += (string condition, string stackTrace, LogType type) =>
        {
            _line++;
            Text.text += condition + "\n";
            Content.sizeDelta = new Vector2(0, 60 + 42 * _line);
            Scrollbar.value = 0;
            // Text
        };
        Debug.Log("test");
    }

    // Update is called once per frame
    void Update()
    {

    }
}
