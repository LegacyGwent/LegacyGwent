using Autofac;
using Cynthia.Card.Client;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SwitchCardEvent : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public CardShowInfo CardShowInfo;
    public MainCodeService MainCodeService { get; set; }

    private void Start()
    {
        MainCodeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        MainCodeService.ClickSwitchUICard(CardShowInfo.CurrentCore);
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f * 1.05f, 0.1f);
        // MainCodeService.SelectSwitchUICard(CardShowInfo.CurrentCore);
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f, 0.1f);
        // MainCodeService.SelectSwitchUICard(CardShowInfo.CurrentCore, false);
    }
}
