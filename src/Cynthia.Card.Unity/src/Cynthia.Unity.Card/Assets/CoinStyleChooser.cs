using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using Assets.Script.Localization;
using Autofac;

public class CoinStyleChooser : MonoBehaviour
{
    public Text ShowText;
    public Text MenuTitle;
    private LocalizationService translator;

    private List<string> _coinOptions = new List<string> { "CoinSeparate", "CoinAdded" };
    private string _selectedOption;

    [Serializable]
    public class CoinStyleEvent : UnityEvent<string> { }
    public CoinStyleEvent onValueChanged = new CoinStyleEvent();

    private void Start()
    {
        _selectedOption = PlayerPrefs.GetString("CoinDisplayMode", _coinOptions[0]);
        translator = DependencyResolver.Container.Resolve<LocalizationService>();
        MenuTitle.text = translator.GetText("CoinStyle");
        ShowText.text = translator.GetText(_selectedOption);
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
            ShowText.text = translator.GetText(_selectedOption);

        Debug.Log("[CoinStyleChooser] Selected: " + _selectedOption);
        onValueChanged.Invoke(_selectedOption);
    }

    public string GetSelectedOption()
    {
        return _selectedOption;
    }
}