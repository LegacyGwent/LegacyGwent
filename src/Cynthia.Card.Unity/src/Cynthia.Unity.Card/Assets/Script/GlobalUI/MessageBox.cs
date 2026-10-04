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
        RemoveTimerBar();
        Buttons.SetActive(false);
        TitleText.text = _translator.GetText(title);
        MessageText.text = _translator.GetText(message);
    }
    // Show a message without buttons that closes itself after the given seconds,
    // optionally with a copy of the turn timer bar (the rope) counting down
    public void ShowAutoClose(string title, string message, float seconds, bool showTimer = false)
    {
        //正在等待点击的消息框先当作确认,避免它的等待者收到之后别的消息框的点击
        if (gameObject.activeSelf && Buttons.activeSelf)
            sender.SendAsync<bool>(true);
        Wait(title, message);
        if (showTimer)
            CreateTimerBar(seconds);
        StartCoroutine(CountdownAndClose(_showCount, seconds));
    }
    private IEnumerator CountdownAndClose(int showCount, float seconds)
    {
        var left = seconds;
        while (left > 0)
        {
            if (showCount != _showCount)
                yield break;
            UpdateTimerBar(left);
            yield return null;
            left -= Time.unscaledDeltaTime;
        }
        if (showCount == _showCount)
            Close();
    }

    private GameObject _timerBar;
    private Slider _timerSlider;
    private Text _timerCount;
    private Transform _timerWolf;
    // Copies the rope of the game scene into the message box, in the place of the buttons
    private void CreateTimerBar(float seconds)
    {
        var ropeController = FindObjectOfType<RopeController>();
        if (ropeController == null)
            return;
        _timerBar = new GameObject("TimerBar", typeof(RectTransform));
        var barTransform = (RectTransform)_timerBar.transform;
        barTransform.SetParent(Context, false);
        barTransform.sizeDelta = new Vector2(Context.sizeDelta.x, 60);
        barTransform.SetSiblingIndex(Buttons.transform.GetSiblingIndex());
        var rope = Instantiate(ropeController.rope.gameObject, barTransform, false);
        rope.SetActive(true);
        var ropeTransform = (RectTransform)rope.transform;
        ropeTransform.anchorMin = ropeTransform.anchorMax = new Vector2(0.5f, 0.5f);
        ropeTransform.anchoredPosition = Vector2.zero;
        ropeTransform.localScale = new Vector3(0.75f, 0.75f, 1);
        _timerSlider = rope.GetComponent<Slider>();
        _timerSlider.interactable = false;
        _timerSlider.minValue = 0;
        _timerSlider.maxValue = seconds;
        _timerCount = rope.transform.Find("TimeCount")?.GetComponent<Text>();
        _timerWolf = rope.transform.Find("WolfIcon");
        LayoutRebuilder.ForceRebuildLayoutImmediate(Context);
    }
    // Same look as RopeController: the bar shrinks, the seconds count down and the wolf shakes
    private void UpdateTimerBar(float left)
    {
        if (_timerBar == null)
            return;
        _timerSlider.value = left;
        if (_timerCount != null)
            _timerCount.text = ((int)(left + 0.5)).ToString();
        if (_timerWolf != null)
            _timerWolf.eulerAngles = new Vector3(0, 0, Random.Range(-1, 1));
    }
    private void RemoveTimerBar()
    {
        if (_timerBar == null)
            return;
        Destroy(_timerBar);
        _timerBar = null;
    }
    public void Close()
    {
        RemoveTimerBar();
        gameObject.SetActive(false);
        Buttons.SetActive(true);
    }
    public Task<bool> Show(string title, string message, string yes = "PopupWindow_YesButton", string no = "PopupWindow_NoButton", bool isOnlyYes = false)
    {
        gameObject.SetActive(true);
        _showCount++;
        RemoveTimerBar();
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
