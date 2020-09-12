using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using Alsein.Extensions.IO;
using Assets.Script.Localization;
using Autofac;

public class MessageBox : MonoBehaviour
{
    public Text TitleText;
    public Text MessageText;
    public Text YesText;
    public Text NoText;
    public GameObject YesButton;
    public GameObject NoButton;
    public GameObject Buttons;
    public ITubeInlet sender;
    public ITubeOutlet receiver;
    //private IAsyncDataSender sender;
    //private IAsyncDataReceiver receiver;
    public RectTransform Context;

    private ITranslator _translator;
    private void Awake()
    {
        (sender, receiver) = Tube.CreateSimplex();
        _translator = DependencyResolver.Container.Resolve<ITranslator>();
    }
    public void Wait(string title, string message)
    {
        Buttons.SetActive(false);
        TitleText.text = _translator.GetText(title);
        MessageText.text = _translator.GetText(message);
        gameObject.SetActive(true);
    }
    public void Close()
    {
        gameObject.SetActive(false);
        Buttons.SetActive(true);
    }
    public Task<bool> Show(string title, string message, string yes = "PopupWindow_YesButton", string no = "PopupWindow_NoButton", bool isOnlyYes = false)
    {
        gameObject.SetActive(true);
        Buttons.SetActive(true);
        if (isOnlyYes)
        {
            YesButton.SetActive(true);
            NoButton.SetActive(false);
        }
        else
        {
            YesButton.SetActive(true);
            NoButton.SetActive(true);
        }
        TitleText.text = _translator.GetText(title);
        MessageText.text = _translator.GetText(message);
        YesText.text = _translator.GetText(yes);
        NoText.text = _translator.GetText(no);
        // LayoutRebuilder.ForceRebuildLayoutImmediate(Context);
        return receiver.ReceiveAsync<bool>();
    }
    public void YesClick()
    {
        sender.SendAsync<bool>(true);
        gameObject.SetActive(false);
    }
    public void NoClick()
    {
        sender.SendAsync<bool>(false);
        gameObject.SetActive(false);
    }
}
