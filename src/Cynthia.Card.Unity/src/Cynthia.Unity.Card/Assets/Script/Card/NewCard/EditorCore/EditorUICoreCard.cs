using Autofac;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorUICoreCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const float StableScale = 1.76f;
    public CardShowInfo cardShowInfo;
    private MainCodeService _mainCodeService;
    public GameObject CountIcon;
    public GameObject Gray;
    public Text CountText;
    public int Count
    {
        get => _count;
        set
        {
            if (_count == value) return;
            _count = value;
            Gray.SetActive(false);
            CountIcon.SetActive(false);
            if (_count > 1)
            {   //如果数量大于1,设定并显示数量
                CountIcon.SetActive(true);
                CountText.text = $"X{value}";
            }
            else if (_count <= 0)
            {   //如果小于等于0,灰
                Gray.SetActive(true);
            }
        }
    }
    private int _count = -1;

    private void Awake()
    {
        var rect = transform as RectTransform;
        if (rect != null) rect.localScale = new Vector3(StableScale, StableScale, 1f);
        TryResolveMainCodeService();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        var show = cardShowInfo != null ? cardShowInfo : gameObject.GetComponent<CardShowInfo>();
        // A grey card is informational only. Do not send an impossible edit to
        // the editor/server and do not punish the player with an error popup.
        if (Count <= 0 || show?.CurrentCore == null || !TryResolveMainCodeService()) return;
        _mainCodeService.ClickEditorUICoreCard(show.CurrentCore);
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        var show = cardShowInfo != null ? cardShowInfo : gameObject.GetComponent<CardShowInfo>();
        if (show?.CurrentCore == null || !TryResolveMainCodeService()) return;
        _mainCodeService.SelectSwitchUICard(show.CurrentCore);
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        var show = cardShowInfo != null ? cardShowInfo : gameObject.GetComponent<CardShowInfo>();
        if (show?.CurrentCore == null || !TryResolveMainCodeService()) return;
        _mainCodeService.SelectSwitchUICard(show.CurrentCore, false);
    }

    private bool TryResolveMainCodeService()
    {
        if (_mainCodeService != null) return true;
        try
        {
            _mainCodeService = DependencyResolver.Container?.Resolve<MainCodeService>();
        }
        catch
        {
            _mainCodeService = null;
        }
        return _mainCodeService != null;
    }
}
