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

    internal  LocalizationService _translator;
    //每次显示新内容时递增,自动关闭只关闭自己显示的那条
    private int _showCount;
    protected void Awake()
    {
        (sender, receiver) = Tube.CreateSimplex();
        _translator = DependencyResolver.Container.Resolve<LocalizationService>();
    }
    public void Wait(string title, string message)
    {
        //先激活,保证Awake已执行(_translator已初始化)
        gameObject.SetActive(true);
        _showCount++;
        Buttons.SetActive(false);
        TitleText.text = _translator.GetText(title);
        MessageText.text = _translator.GetText(message);
    }
    // Show a message without buttons that closes itself after the given seconds
    public void ShowAutoClose(string title, string message, float seconds)
    {
        //正在等待点击的消息框先当作确认,避免它的等待者收到之后别的消息框的点击
        if (gameObject.activeSelf && Buttons.activeSelf)
            sender.SendAsync<bool>(true);
        Wait(title, message);
        StartCoroutine(CloseAfter(_showCount, seconds));
    }
    private IEnumerator CloseAfter(int showCount, float seconds)
    {
        yield return new WaitForSecondsRealtime(seconds);
        if (showCount == _showCount)
            Close();
    }
    public void Close()
    {
        gameObject.SetActive(false);
        Buttons.SetActive(true);
    }
    public Task<bool> Show(string title, string message, string yes = "PopupWindow_YesButton", string no = "PopupWindow_NoButton", bool isOnlyYes = false)
    {
        gameObject.SetActive(true);
        _showCount++;
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
    public virtual void YesClick()
    {
        sender.SendAsync<bool>(true);
        gameObject.SetActive(false);
    }
    public virtual void NoClick()
    {
        sender.SendAsync<bool>(false);
        gameObject.SetActive(false);
    }
}
