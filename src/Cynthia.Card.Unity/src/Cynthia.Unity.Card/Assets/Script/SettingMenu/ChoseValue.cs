using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoseValue : MonoBehaviour {
    public string[] ChoseList;
    private int _index=0;
    public int Index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;
            ShowText.text = ChoseList[Index];
            onValueChanged.Invoke(Index);
        }
    }
    [Serializable]
    public class ChoseValueEvent : UnityEvent<int>
    { }
    public ChoseValueEvent onValueChanged = new ChoseValueEvent();

    public Text ShowText;

    private void Start()
    {
        ShowText.text = ChoseList[Index];
    }

    public void LeftButtonClick()
    {
        if (Index-1 < 0)
        {
            Index = ChoseList.Length - 1;
            return;
        }
        Index--;
    }
    public void RightButtonClick()
    {
        if (Index+1 >= ChoseList.Length)
        {
            Index = 0;
            return;
        }
        Index++;
    }
}
