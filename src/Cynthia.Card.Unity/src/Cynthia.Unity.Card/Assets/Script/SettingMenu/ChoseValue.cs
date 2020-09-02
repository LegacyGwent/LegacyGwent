using System;
using System.Collections;
using System.Collections.Generic;
using Autofac;
using Cynthia.Card.Common.Models;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ChoseValue : MonoBehaviour {
    public List<string> ChoseList;
    private int _index = 0;
    public int Index
    {
        get
        {
            return _index;
        }
        set
        {
            _index = value;
            if (_translator == null) { Start(); }
            ShowText.text = _translator.GetText(ChoseList[Index]);
            onValueChanged.Invoke(Index);
        }
    }
    [Serializable]
    public class ChoseValueEvent : UnityEvent<int>
    { }
    public ChoseValueEvent onValueChanged = new ChoseValueEvent();

    public Text ShowText;

    private ITranslator _translator;

    private void Start()
    {
        _translator = DependencyResolver.Container.Resolve<ITranslator>();
    }

    public void LeftButtonClick()
    {
        if (Index-1 < 0)
        {
            Index = ChoseList.Count - 1;
            return;
        }
        Index--;
    }
    public void RightButtonClick()
    {
        if (Index+1 >= ChoseList.Count)
        {
            Index = 0;
            return;
        }
        Index++;
    }
}
