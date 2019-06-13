using Autofac;
using Cynthia.Card.Client;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EditorUIShowCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public GameCodeService GameCodeService { get; set; }

    private void Start()
    {
        //GameCodeService = DependencyResolver.Container.Resolve<GameCodeService>();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        //GameCodeService.ClickUICard(transform.GetSiblingIndex());
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f * 1.04f, 0.1f);
        //.localScale *= 1.05f;
        //GameCodeService.SelectUICard(transform.GetSiblingIndex());
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f, 0.1f);
        ///= 1.05f;
        //GameCodeService.SelectUICard(-1);
    }
}
