using Autofac;
using Cynthia.Card.Client;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class EditorUICoreCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
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

    private void Start()
    {
        _mainCodeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        _mainCodeService.ClickEditorUICoreCard(gameObject.GetComponent<CardShowInfo>().CurrentCore);
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f * 1.05f, 0.1f);
        _mainCodeService.SelectSwitchUICard(gameObject.GetComponent<CardShowInfo>().CurrentCore);
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f, 0.1f);
        _mainCodeService.SelectSwitchUICard(gameObject.GetComponent<CardShowInfo>().CurrentCore, false);
    }
}
