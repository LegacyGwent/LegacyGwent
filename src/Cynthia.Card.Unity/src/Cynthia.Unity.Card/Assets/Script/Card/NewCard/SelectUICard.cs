using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Cynthia.Card.Client;
using DG.Tweening;
using Autofac;

public class SelectUICard : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerClickHandler
{
    public GameCodeService GameCodeService { get; set; }
    public GameObject SelectIcon;
    public bool IsSelect { get => _isSelect;
        set
        {
            _isSelect = value;
            SelectIcon.SetActive(value);
        }
    }
    private bool _isSelect;

    private void Start()
    {
        GameCodeService = DependencyResolver.Container.Resolve<GameCodeService>();
    }
    //鼠标点击
    public void OnPointerClick(PointerEventData eventData)
    {
        #if UNITY_ANDROID || UNITY_IOS
            GameCodeService.ClickUICard(transform.GetSiblingIndex());
        #else
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                GameCodeService.ClickUICard(transform.GetSiblingIndex());
            }
        #endif
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f*1.05f,0.1f);
            //.localScale *= 1.05f;
        GameCodeService.SelectUICard(transform.GetSiblingIndex());
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        gameObject.GetComponent<RectTransform>().DOScale(1.77f, 0.1f);
            ///= 1.05f;
        GameCodeService.SelectUICard(-1);
    }
}
