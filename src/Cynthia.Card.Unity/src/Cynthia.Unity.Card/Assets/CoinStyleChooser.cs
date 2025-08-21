using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class CoinStyleChooser : MonoBehaviour
{
    public Text ShowText;

    private List<string> _coinOptions = new List<string> { "CoinSeparate", "CoinAdded" };
    private string _selectedOption;

    [Serializable]
    public class CoinStyleEvent : UnityEvent<string> { }
    public CoinStyleEvent onValueChanged = new CoinStyleEvent();

    private void Start()
    {
        // Initialize with the first option or saved option
        _selectedOption = PlayerPrefs.GetString("CoinDisplayMode", _coinOptions[0]);
        ShowText.text = _selectedOption;
        onValueChanged.Invoke(_selectedOption);
    }

    public void LeftButtonClick()
    {
        int currentIndex = _coinOptions.IndexOf(_selectedOption);
        currentIndex = (currentIndex - 1 + _coinOptions.Count) % _coinOptions.Count;
        SetOption(_coinOptions[currentIndex]);
    }

    public void RightButtonClick()
    {
        int currentIndex = _coinOptions.IndexOf(_selectedOption);
        currentIndex = (currentIndex + 1) % _coinOptions.Count;
        SetOption(_coinOptions[currentIndex]);
    }

    private void SetOption(string option)
    {
        _selectedOption = option;
        if (ShowText != null)
            ShowText.text = _selectedOption;

        Debug.Log("[CoinStyleChooser] Selected: " + _selectedOption);
        onValueChanged.Invoke(_selectedOption);
    }

    public string GetSelectedOption()
    {
        return _selectedOption;
    }
}