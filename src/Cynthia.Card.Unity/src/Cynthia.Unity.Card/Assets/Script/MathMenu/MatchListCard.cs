using System.Collections;
using System.Collections.Generic;
using Autofac;
using Cynthia.Card.Client;
using UnityEngine;
using UnityEngine.EventSystems;

public class MatchListCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    private MainCodeService _mainCodeService;
    private void Start()
    {
        _mainCodeService = DependencyResolver.Container.Resolve<MainCodeService>();
    }
    //鼠标进入
    public void OnPointerEnter(PointerEventData eventData)
    {
        _mainCodeService.SetMatchArtCard(gameObject.GetComponent<ListCardShowInfo>().CardStatus);
    }
    //鼠标离开
    public void OnPointerExit(PointerEventData eventData)
    {
        _mainCodeService.SetMatchArtCard(gameObject.GetComponent<ListCardShowInfo>().CardStatus, false);
    }
}
